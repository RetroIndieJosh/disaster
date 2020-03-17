# Duality Jam

## Bugs

## Features

- IsPlayable implementation
        - Board.HasControlledDisaster
        - Board.HasClearSpace
        - Board.HasStone(color)
                - also for elimination game over
- disallow water turning back on itself
- quelch fire if touching fire on two sides
- skip first turn auto advance?
        - spread card: as two step cards
        - advancement onto water stops
- plague card: select placement (occupied), select facing
- wild card: select which card to copy, use that logic
- currently can't turn with extend/spread, is this good?
        - can only turn with disaster card

- disable change of disaster if trail < 3
- auto advancement (enable with checkbox)
        - choose new facing on auto advancement
- fire/plague/water as turn cards
- spread can be used as turn (or turn as its own card)

- card info from text file

- CARD COUNT MUST BE 2n+3
