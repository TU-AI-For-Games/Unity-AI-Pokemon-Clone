# Build a dictionary of MOVE_NAME:ID
moves_filepath = "C:\\Users\\tomsc\\Documents\\AI-for-Games-ICA\\Assets\\Resources\\Data\\moves.csv"
moves = {}
with open(moves_filepath, 'r') as moves_file:
    moves_data = moves_file.readlines()

for i in range(1, len(moves_data)):
    contents = moves_data[i].split(',')
    moves[contents[1].upper()] = contents[0]

print(f"Found {len(moves)} moves...")

# Replace the move names from the original CSV file with the move IDs from the moves csv
pkmn_filepath = "C:\\Users\\tomsc\\Documents\\AI-for-Games-ICA\\Assets\\Resources\\Data\\pokemon.csv"
with open(pkmn_filepath, 'r') as pkmn_file:
    pkmn_data = pkmn_file.readlines()

for i in range(1, len(pkmn_data)):
    line = pkmn_data[i].split(',')
    for j in range(7, 11):
        if "\n" in line[j]: line[j] = line[j][:-1]
        line[j] = moves[line[j].upper()]
    pkmn_data[i] = ",".join(line) + "\n"

# Write the new data to the file
with open(pkmn_filepath, 'w') as pkmn_file:
    pkmn_file.writelines(pkmn_data)


