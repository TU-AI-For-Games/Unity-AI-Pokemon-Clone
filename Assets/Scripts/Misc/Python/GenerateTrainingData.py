
oldFile = "..\\..\\..\\Resources\\Data\\typeAdvantages.csv"

typeAdvantages = {}

allTypes = [
    "Bug",
    "Dark",
    "Dragon",
    "Electric",
    "Fire",
    "Fighting",
    "Flying",
    "Ghost",
    "Grass",
    "Ground",
    "Ice",
    "Normal",
    "Poison",
    "Psychic",
    "Rock",
    "Steel",
    "Water"
]

with open(oldFile, 'r') as typeAdvantageFile:
    lines = typeAdvantageFile.readlines()

for i in range(1, len(lines)):
    line = lines[i].split(',')
    type = line[1]

    if type not in typeAdvantages.keys():
        typeAdvantages[type] = {}

    typeAdvantages[type][line[2]] = float(line[3][:-1])

# Add all the neutrals into the dataset
for typeFromFile in typeAdvantages.values():
    for type in allTypes:
        if type not in typeFromFile.keys():
            typeFromFile[type] = 1.0


# Write out to a new file
outFile = "..\\..\\..\\Resources\\Data\\AI_Training\\typing.csv"
with open(outFile, 'w') as typeAdvantageFile:
    typeAdvantageFile.write(','.join(allTypes) + ",Immune,Not Very Effective,Neutral,Super Effective\n")
    for type in typeAdvantages.keys():
        for counterType in typeAdvantages[type].keys():
            dataToWrite = ','.join(["1" if type == x or counterType == x else "0" for x in allTypes])

            if typeAdvantages[type][counterType] == 2.0:
                dataToWrite += ",0,0,0,1"
            elif typeAdvantages[type][counterType] == 1.0:
                dataToWrite += ",0,0,1,0"
            elif typeAdvantages[type][counterType] == 0.5:
                dataToWrite += ",0,1,0,0"
            else:
                dataToWrite += ",1,0,0,0"

            typeAdvantageFile.write(dataToWrite + "\n")


