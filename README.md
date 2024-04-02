## Checkers game with a graphical interface based on neural networks. The neural network is built on training perceptrons using a genetic algorithm.
1.In the folder "checkers_train", perceptrons are trained. During training, each move is predicted 3 moves ahead: move - enemy move - move.The weights of the best perceptron are written to a file.
First you need to run this solution and wait for the algorithm to finish training and write the weights to a file. Now training consists only of the number of iterations, they can be reduced.
1.In the folder "checkers" the code with the implementation of the game graphics: a board with the ability to play against a neural network. The neural network, before responding to your move, also analyzes 3 moves ahead.
The right mouse button is to select a checker, the left button is to select a move location. The interface will tell you where you can’t go and when you need to eat.
