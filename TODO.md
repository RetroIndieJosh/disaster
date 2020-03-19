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
- remove duplicates from initial stones
- extend says it's unplayable after disaster ends but it's still playable

## Features

### Minimum Viable

- IsPlayable implementation
        - Board.HasStone(color)
                - also for elimination game over

### Important

- disable change of disaster if trail < 3
- quelch fire if touching fire on two sides
- disallow water/fire turning back on itself
- wild card: select which card to copy, use that logic

### Wishlist

- plague card: select placement (occupied), select facing
- card info from text file

- CARD COUNT MUST BE 2n+3
