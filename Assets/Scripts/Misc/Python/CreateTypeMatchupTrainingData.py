
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
    for type in typeAdvantages.keys():
        for counterType in typeAdvantages[type].keys():
            typeAdvantageFile.write(f"{allTypes.index(type)},{allTypes.index(counterType)},{typeAdvantages[type][counterType]}\n")


