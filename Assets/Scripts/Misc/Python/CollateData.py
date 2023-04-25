import os, math

def calculate_hp_stat(base, iv, ev, level):
    return int(((base + iv) * 2 + math.sqrt(ev) / 4) * level / 100) + level + 10;


def load_pokemon_dictionary():
    # ID : HEALTH
    pokemon = dict()

    # Load the data about the pokemon's HP
    with open("..\\..\\..\\Resources\\Data\\pokemon.csv", 'r') as pokemon_data:
        lines = pokemon_data.readlines()

    for i in range(1, len(lines)):
        mon = lines[i].split(',')
        pokemon[int(mon[0])] = calculate_hp_stat(int(mon[3]), 15, 256, 50)
    return pokemon


def normalise_data(battleData):
    data = battleData.split(',')
    # Player Pkmn ID,Target Pkmn ID,Player HP,Target HP,Player Element,Target Element,Attack?,Switch?,Heal?
    playerId = int(data[0]) / 151
    targetId = int(data[2]) / 151
    playerHealth = int(data[2]) / pokeData[int(data[0])]
    targetHealth = int(data[3]) / pokeData[int(data[1])]
    playerElement = int(data[4]) / 17
    targetElement = int(data[5]) / 17
    dataString = f"{playerId},{targetId},{playerHealth},{targetHealth},{playerElement},{targetElement},{','.join(data[-3:])}"
    return dataString


def generate_output_line(battleData):
    # Chosen Action (Attack/Switch/Heal),Player Pkmn ID,Target Pkmn ID,Player HP,Target HP,Player Element,Target Element,Previous Pkmn ID,Previous Pkmn Element,Chosen Move ID,Move Element,STAB?,Move Power,User Status,Status Move?,Status Hit?,Status Applied,Hit?,Effectiveness(0-2),Stat Change?,Stat Changed,Stat Target(T/U),Kill?,Outcome Target HP
    data = battleData.split(',')

    # Player HP,Target HP,Player Element,Target Element,Attack?,Switch?,Heal?
    outcome = ["0", "0", "0"]
    outcome[int(data[0])] = "1"

    dataString = normalise_data(f"{','.join(data[1:7])},{','.join(outcome)}")
    return dataString + '\n'


def parse_battle_data_file(filename, outFile):
    with open(filename, 'r') as battleDataFile:
        lines = battleDataFile.readlines()

    for i in range(1, len(lines)):
        outFile.write(generate_output_line(lines[i]))


pokeData = load_pokemon_dictionary()

data_folder = "C:\\Users\\tomsc\\AppData\\Roaming\\AiPokeClone"

output_file = open("..\\..\\..\\Resources\\Data\\AI_Training\\decisionMakingData.csv", 'w')

output_file.write("Player Pkmn ID,Target Pkmn ID,Player HP,Target HP,Player Element,Target Element,Attack?,Switch?,Heal?\n")

for root, dirs, files in os.walk(data_folder):
    for file in files:
        parse_battle_data_file(os.path.join(root, file), output_file)

output_file.close()



