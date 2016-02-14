import csv

adjacentList = {}
positionList = {}
edgeVisitedList = {}
path = [];
stack = [];
with open('CS3630_Lab2_Map2.csv', 'r') as csvfile:
	myReader = csv.reader(csvfile, delimiter = ',')
	for row in myReader:
		if (len(row) == 3):
			positionList[row[0]] = (float(row[1]), float(row[2]));
			adjacentList[row[0]] = [];
		if (len(row) == 2):
			adjacentList[row[0]].append(row[1]);
			adjacentList[row[1]].append(row[0]);
			edgeVisitedList[(row[0], row[1])] = False;
startNode = adjacentList.keys()[0];
for node in adjacentList:
	if (len(adjacentList[node]) % 2 != 0):
		startNode = node
		break
prev = None
stack.append((None, node))
path = []
insertPos = 0;
while (len(stack) != 0):
	curr = stack.pop()
	visited = False
	if edgeVisitedList.has_key(curr):
		visited = edgeVisitedList[curr]
		edgeVisitedList[curr] = True
	elif edgeVisitedList.has_key((curr[1], curr[0])):
		visited = edgeVisitedList[(curr[1], curr[0])]
		edgeVisitedList[(curr[1], curr[0])] = True
	if not visited:
		for next in adjacentList[curr[1]]:
			if edgeVisitedList.has_key((curr[1], next)):
				if (not edgeVisitedList[(curr[1], next)]):
					stack.append((curr[1], next))
			else:
				if (not edgeVisitedList[(next, curr[1])]):
					stack.append((curr[1], next))
		if (len(path) > 0):
			if (path[insertPos - 1] != curr[0]):
				for pos in range(0, len(path)):
					if (path[pos] == curr[0]):
						insertPos = pos + 1
						break
		path.insert(insertPos, curr[1])
		insertPos = insertPos + 1
print path
positionPath = []
for node in path:
	positionPath.append(positionList[node])
print positionPath




