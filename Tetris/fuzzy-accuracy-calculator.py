import json

def calculate_accuracy():
    with open('Tetris Database.json', 'r') as file:
        data = json.load(file)

    total_documents = len(data)
    accurate_guesses = 0
    no_fuzzy_output_count = 0

    for entry in data:
        human_output = entry["Rotation"]["HumanOutput"]
        fuzzy_output = entry["Rotation"]["FuzzyOutput"]

        if fuzzy_output is not None:
            if human_output == fuzzy_output:
                accurate_guesses += 1
        else:
            no_fuzzy_output_count += 1

    success_rate = (accurate_guesses / (total_documents - no_fuzzy_output_count)) * 100 if total_documents > 0 else 0

    print(f"Number of reviewed documents: {total_documents}")
    print(f"Number of accurate guesses by the fuzzy system: {accurate_guesses}")
    print(f"Success rate of the fuzzy system: {success_rate:.2f}%")
    print(f"Number of documents with no output by the fuzzy system: {no_fuzzy_output_count}")

calculate_accuracy()