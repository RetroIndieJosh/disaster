# Duality Jam

Extend - silver
Fire - red
Life - green
Move - gold
Plague - yellow
Spread - gray
Water - blue
Wild (?) - purple

# Rules

- disaster acts like spread for itself or extend for any other
- can only create with disaster
- can extend/spread either player's controlled disaster (then why does it matter who controls it?)

# Design Questions

- what happens if entire hand is unplayable? (spread/extend with no active disaster)
- extend and especially spread are OP if they can both move and turn any disaster
        - maybe neither changes direction after advancing

## Bugs

- after extending/spreading to a stop point, shouldn't ask for direction (ret true)
- disallow moving trapped piece
- remove duplicates from initial stones

## Features

### Minimum Viable

- IsPlayable implementation
        - Board.HasStone(color)
                - also for elimination game over
- remove pieces from disasters that are >3 away from head
- auto advance (skip placement turn?)
        - choose new facing on auto advancement?

### Important

- disable change of disaster if trail < 3
- quelch fire if touching fire on two sides
- disallow water/fire turning back on itself
- wild card: select which card to copy, use that logic
- currently can't turn with extend/spread, is this good?
        - can only turn with disaster card

### Wishlist

- plague card: select placement (occupied), select facing
- card info from text file

- CARD COUNT MUST BE 2n+3
