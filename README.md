# Minesweeper
A text-based implementation of Minesweeper for dotnetcore 1.1

This sounded like a fun little challenge, so I put this together in my spare time over a couple days. 

You specify the size of the N by N 
board, the number of bombs you would like it to hide, then the game begins. You may (S)how the contents of a cell, (F)lag a cell 
as suspected of containing a bomb, or (Q)uit. If you Show or Flag, you will need to enter the X coordinate (column #) followed by the 
Y coordinate (row #).

As you uncover cells, you will see numbers revealed. A number means that there are that number of bombs touching that cell (both sides,
top/bottom, and diagonally). If you see a 1 on the board, it means that cell is touching exactly 1 mine. Use all of the numbers in a 
given area to figure out where the mines are.

If you Show a cell containing a bomb, you lose.
