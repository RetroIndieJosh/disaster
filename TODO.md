# Duality Jam

## Bugs

## Features

        - choose new facing after advancement
        - disable change of disaster if trail < 3
        - keep a list of inactive disasters to step at end of turn from Board
        - count kills for game result (if tied on life)
- skip first turn auto advance?
- step card: auto-move active disaster, select facing
        - spread card: as two step cards
        - advancement onto water stops
        - advancement onto stone kills stone
        - water card when active disaster is water: as spread
- disable step/spread if no active disaster
- fire card: as water but double speed
        - water card when active disaster is fire: as step
        - fire card when active disaster is fire: as spread
        - fire card when active disaster is water: as step
- fire stops at water
- water crosses through fire
- plague card: select placement (occupied), select facing
        - water card when active disaster is plague: as step
        - fire card when active disaster is plague: as step
        - plague card when active disaster is plague: as spread
        - plague card when active disaster is water: as step
        - plague card when active disaster is fire: as step
- wild card: select which card to copy, use that logic

- card info from text file
- more modular setup for card/board actions

- CARD COUNT MUST BE 2n+3
