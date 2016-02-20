// Copyright (c) Microsoft Corporation.  All rights reserved.

//------------------------------------------------------------------------------
// ToneGenerator.cs
//
//     This code was generated by the DssNewService tool.
//
//------------------------------------------------------------------------------

using Microsoft.Ccr.Core;
using Microsoft.Dss.Core;
using Microsoft.Dss.Core.Attributes;
using Microsoft.Dss.ServiceModel.Dssp;
using Microsoft.Dss.ServiceModel.DsspServiceBase;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml;
using W3C.Soap;
using Myro.Utilities;

using brick = Myro.Services.Scribbler.ScribblerBase.Proxy;
using vector = Myro.Services.Generic.Vector;

namespace Myro.Services.Scribbler.ToneGenerator
{
    public static class Contract
    {
        public const string Identifier = "http://www.roboteducation.org/schemas/2008/06/scribblertonegenerator.html";
    }

    /// <summary>
    /// The Tone Generator Service
    /// </summary>
    [DisplayName("Scribbler Tone Generator")]
    [Description("The Scribbler ToneGenerator Service")]
    [Contract(Contract.Identifier)]
    [AlternateContract(vector.Contract.Identifier)] //implementing the generic contract
    public class ScribblerToneGenerator : vector.VectorServiceBase
    {
        [ServicePort(AllowMultipleInstances = false)]
        vector.VectorOperations _operationsPort = new vector.VectorOperations();
        protected override vector.VectorOperations OperationsPort { get { return _operationsPort; } }

        /// <summary>
        /// Robot base partner
        /// </summary>
        [Partner("ScribblerBase", Contract = brick.Contract.Identifier,
            CreationPolicy = PartnerCreationPolicy.UseExistingOrCreate, Optional = false)]
        private brick.ScribblerOperations _scribblerPort = new brick.ScribblerOperations();

        /// <summary>
        /// Constructor
        /// </summary>
        public ScribblerToneGenerator(DsspServiceCreationPort creationPort)
            : base(creationPort)
        {
            _state = new vector.VectorState(
                new List<double> { 0.0, 0.0, 0.0, 1.0 },
                new List<string> { "tone1", "tone2", "duration", "loud" },
                DateTime.Now);
        }

        /// <summary>
        /// Actuator callback
        /// </summary>
        protected override IEnumerator<ITask> SetCallback(Myro.Services.Generic.Vector.SetRequestInfo request, PortSet<vector.CallbackResponseType, Fault> responsePort)
        {
            var req = request as vector.SetElementsRequestInfo;
            if (req != null)
            {
                bool play = false;
                bool loud = false;
                foreach (var i in req.Indices)
                    if (i == 0 || i == 1 || i == 2)
                        play = true;
                    else if (i == 3)
                        loud = true;

                Fault error = null;
                if (loud)
                    yield return Arbiter.Choice(setLoud(),
                        delegate(vector.CallbackResponseType s) { },
                        delegate(Fault f) { error = f; });

                if (error == null && play)
                    yield return Arbiter.Choice(playTone(),
                        delegate(vector.CallbackResponseType s1) { },
                        delegate(Fault f) { error = f; });

                if (error == null)
                    responsePort.Post(vector.CallbackResponseType.Instance);
                else
                    responsePort.Post(error);
            }
            else
            {
                // Otherwise it was a SetAllRequestInfo
                Activate(Arbiter.Choice(setLoud(),
                    delegate(vector.CallbackResponseType s)
                    {
                        Activate(Arbiter.Choice(playTone(),
                            delegate(vector.CallbackResponseType s1) { responsePort.Post(vector.CallbackResponseType.Instance); },
                            delegate(Fault f) { responsePort.Post(f); }));
                    },
                    delegate(Fault f) { responsePort.Post(f); }));
            }
            yield break;
        }

        private DsspResponsePort<vector.CallbackResponseType> playTone()
        {
            var responsePort = new DsspResponsePort<vector.CallbackResponseType>();
            brick.PlayToneBody play = new brick.PlayToneBody()
            {
                Frequency1 = (int)Math.Round(_state.Values[0]),
                Frequency2 = (int)Math.Round(_state.Values[1]),
                Duration = _state.Values[2]
            };
            if (play.Frequency1 < 0 || play.Frequency2 < 0 || play.Duration < 0)
                responsePort.Post(RSUtils.FaultOfException(new ArgumentOutOfRangeException()));
            else
                Activate(Arbiter.Choice(_scribblerPort.PlayTone(play),
                    delegate(DefaultUpdateResponseType success) { responsePort.Post(vector.CallbackResponseType.Instance); },
                    delegate(Fault failure) { responsePort.Post(failure); }));
            return responsePort;
        }

        private DsspResponsePort<vector.CallbackResponseType> setLoud()
        {
            var responsePort = new DsspResponsePort<vector.CallbackResponseType>();
            Activate(Arbiter.Choice(_scribblerPort.SetLoud(_state.GetBool(3)),
                delegate(DefaultUpdateResponseType success) { responsePort.Post(vector.CallbackResponseType.Instance); },
                delegate(Fault failure) { responsePort.Post(failure); }));
            return responsePort;
        }

    }
}