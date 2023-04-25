import os

def generate_output_line(battleData):
    # Chosen Action (Attack/Switch/Heal),Player Pkmn ID,Target Pkmn ID,Player HP,Target HP,Player Element,Target Element,Previous Pkmn ID,Previous Pkmn Element,Chosen Move ID,Move Element,STAB?,Move Power,User Status,Status Move?,Status Hit?,Status Applied,Hit?,Effectiveness(0-2),Stat Change?,Stat Changed,Stat Target(T/U),Kill?,Outcome Target HP
    data = battleData.split(',')

    # Player HP,Target HP,Player Element,Target Element,Attack?,Switch?,Heal?
    outcome = ["0", "0", "0"]
    outcome[int(data[0])] = "1"

    dataString = f"{','.join(data[1:7])},{','.join(outcome)}\n"
    return dataString


def parse_battle_data_file(filename, outFile):
    with open(filename, 'r') as battleDataFile:
        lines = battleDataFile.readlines()

    for i in range(1, len(lines)):
        outFile.write(generate_output_line(lines[i]))


data_folder = "C:\\Users\\tomsc\\AppData\\Roaming\\AiPokeClone"

output_file = open("..\\..\\..\\Resources\\Data\\AI_Training\\decisionMakingData.csv", 'w')

output_file.write("Player Pkmn ID,Target Pkmn ID,Player HP,Target HP,Player Element,Target Element,Attack?,Switch?,Heal?\n")

for root, dirs, files in os.walk(data_folder):
    for file in files:
        parse_battle_data_file(os.path.join(root, file), output_file)

output_file.close()



