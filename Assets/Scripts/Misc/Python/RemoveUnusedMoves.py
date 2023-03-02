
# Loop through the pokemon and create a set of all of the move IDs
pokemon_filepath = "..\\..\\..\\Resources\\Data\\pokemon.csv"
moves_filepath = "..\\..\\..\\Resources\\Data\\moves.csv"

with open(pokemon_filepath, 'r') as pokemon_file:
    pokemon_data = pokemon_file.readlines()

used_moves = set()

for i in range(1, len(pokemon_data)):
    info = pokemon_data[i][:-1].split(',')
    for j in range(7, 11):
        used_moves.add(int(info[j]))

print(used_moves)

# Remove the unused moves from the moves.csv file
with open(moves_filepath, 'r') as moves_file:
    moves = moves_file.readlines()

used_move_data = [moves[0]]
for i in range(1, len(moves)):
    # If the ID is in the used_moves set then add it to the used_move_data
    if int(moves[i].split(',')[0]) in used_moves:
        used_move_data.append(moves[i])

# Write the used moves back
with open(moves_filepath, 'w') as moves_file:
    moves_file.writelines(used_move_data)