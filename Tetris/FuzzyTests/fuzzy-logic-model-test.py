import sys
import numpy as np
import skfuzzy as fuzz
from skfuzzy import control as ctrl

height = ctrl.Antecedent(np.arange(0, 22, 1), 'Height')
type = ctrl.Antecedent(np.arange(0, 7, 1), 'Type')
gsLeft = ctrl.Antecedent(np.arange(-22, 22, 1), 'gsLeft')
gsRight = ctrl.Antecedent(np.arange(-22, 22, 1), 'gsRight')
rotation = ctrl.Consequent(np.arange(-1, 4, 1), 'Rotation')

height['low'] = fuzz.trapmf(height.universe, [0, 0, 4, 8])
height['medium'] = fuzz.trapmf(height.universe, [4, 8, 14, 18])
height['high'] = fuzz.trapmf(height.universe, [14, 18, 22, 22])

type['O'] = fuzz.trimf(type.universe, [0, 0, 1])
type['I'] = fuzz.trimf(type.universe, [0, 1, 2])
type['T'] = fuzz.trimf(type.universe, [1, 2, 3])
type['L'] = fuzz.trimf(type.universe, [2, 3, 4])
type['J'] = fuzz.trimf(type.universe, [3, 4, 5])
type['S'] = fuzz.trimf(type.universe, [4, 5, 6])
type['Z'] = fuzz.trimf(type.universe, [5, 6, 7])

gsLeft['farBelow'] = fuzz.trimf(gsLeft.universe, [-22, -2, -1])
gsLeft['slightlyBelow'] = fuzz.trimf(gsLeft.universe, [-2, -1, 0])
gsLeft['level'] = fuzz.trimf(gsLeft.universe, [-1, 0, 1])
gsLeft['slightlyAbove'] = fuzz.trimf(gsLeft.universe, [0, 1, 2])
gsLeft['farAbove'] = fuzz.trimf(gsLeft.universe, [1, 2, 22])

gsRight['farBelow'] = fuzz.trimf(gsRight.universe, [-22, -2, -1])
gsRight['slightlyBelow'] = fuzz.trimf(gsRight.universe, [-2, -1, 0])
gsRight['level'] = fuzz.trimf(gsRight.universe, [-1, 0, 1])
gsRight['slightlyAbove'] = fuzz.trimf(gsRight.universe, [0, 1, 2])
gsRight['farAbove'] = fuzz.trimf(gsRight.universe, [1, 2, 22])

rotation['0'] = fuzz.trimf(rotation.universe, [-1, 0, 1])
rotation['90'] = fuzz.trimf(rotation.universe, [0, 1, 2])
rotation['180'] = fuzz.trimf(rotation.universe, [1, 2, 3])
rotation['270'] = fuzz.trimf(rotation.universe, [2, 3, 4])

# Rules for Tetromino Type O
rule1 = ctrl.Rule(type['O'] & ~height['low'], rotation['0'])

# Rules for Tetromino Type I
rule2 = ctrl.Rule(type['I'] & gsLeft['level'] & gsRight['level'] & ~height['low'], rotation['0'])
rule3 = ctrl.Rule(type['I'] & (gsLeft['slightlyAbove'] | gsLeft['farAbove']) & (gsRight['slightlyAbove'] | gsRight['farAbove']) & ~height['low'], rotation['90'])

# Rules for Tetromino Type T
rule4 = ctrl.Rule(type['T'] & gsLeft['level'] & gsRight['level'] & ~height['low'], rotation['0'])
rule5 = ctrl.Rule(type['T'] & (gsLeft['slightlyBelow'] | gsRight['slightlyAbove']) & ~height['low'], rotation['90'])
rule6 = ctrl.Rule(type['T'] & gsLeft['slightlyAbove'] & gsRight['slightlyAbove'] & ~height['low'], rotation['180'])
rule7 = ctrl.Rule(type['T'] & (gsLeft['slightlyAbove'] | gsRight['slightlyBelow']) & ~height['low'], rotation['270'])

# Rules for Tetromino Type L
rule8 = ctrl.Rule(type['L'] & gsLeft['level'] & gsRight['level'] & ~height['low'], rotation['0'])
rule9 = ctrl.Rule(type['L'] & (gsLeft['level'] | gsRight['level']) & ~height['low'], rotation['90'])
rule10 = ctrl.Rule(type['L'] & (gsLeft['slightlyBelow'] | gsLeft['farBelow']) & gsRight['level'] & ~height['low'], rotation['180'])
rule11 = ctrl.Rule(type['L'] & (gsLeft['farAbove'] | gsRight['farBelow']) & ~height['low'], rotation['270'])

# Rules for Tetromino Type J
rule12 = ctrl.Rule(type['J'] & gsLeft['level'] & gsRight['level'] & ~height['low'], rotation['0'])
rule13 = ctrl.Rule(type['J'] & (gsLeft['farBelow'] | gsRight['farAbove']) & ~height['low'], rotation['90'])
rule14 = ctrl.Rule(type['J'] & gsLeft['level'] & (gsRight['slightlyBelow'] | gsRight['farBelow']) & ~height['low'], rotation['180'])
rule15 = ctrl.Rule(type['J'] & (gsLeft['level'] | gsRight['level']) & ~height['low'], rotation['270'])

# Rules for Tetromino Type S
rule16 = ctrl.Rule(type['S'] & gsLeft['level'] & gsRight['slightlyAbove'] & ~height['low'], rotation['0'])
rule17 = ctrl.Rule(type['S'] & (gsLeft['slightlyAbove'] | gsRight['slightlyBelow']) & ~height['low'], rotation['90'])

# Rules for Tetromino Type Z
rule18 = ctrl.Rule(type['Z'] & gsLeft['slightlyAbove'] & gsRight['level'] & ~height['low'], rotation['0'])
rule19 = ctrl.Rule(type['Z'] & (gsLeft['slightlyBelow'] | gsRight['slightlyAbove']) & ~height['low'], rotation['90'])

control_system = ctrl.ControlSystem([rule1, rule2, rule3, rule4, rule5, rule6, rule7, rule8, rule9, rule10, rule11, rule12, rule13, rule14, rule15, rule16, rule17, rule18, rule19])

def infer_rotation(height, type, gsLeft, gsRight):
    simulation = ctrl.ControlSystemSimulation(control_system)

    simulation.input['Height'] = height
    simulation.input['Type'] = type
    simulation.input['gsLeft'] = gsLeft
    simulation.input['gsRight'] = gsRight

    try:
        simulation.compute()
        rotation = simulation.output['Rotation']

        if rotation < 1:
            print(0)
        elif rotation == 1:
            print(1)
        elif 1 < rotation <= 2:
            print(2)
        elif rotation > 2:
            print(3)
    except ValueError:
        print("Total area is zero in defuzzification.")

if __name__ == "__main__":
    height_arg = int(sys.argv[1])
    type_arg = int(sys.argv[2])
    gsLeft_arg = int(sys.argv[3])
    gsRight_arg = int(sys.argv[4])

    infer_rotation(height_arg, type_arg, gsLeft_arg, gsRight_arg)