
hex_avr_fitness = importdata("AvrFitness_hex_old.csv");
hex_worst_fitness = importdata("WorstFitness_hex_old.csv");
hex_best_fitness = importdata("BestFitness_hex_old.csv");

hold on
plot(hex_best_fitness, Color='green')
plot(hex_avr_fitness, Color='blue')
plot(hex_worst_fitness, Color='red')
title("Parameter Estimation Hexapod - Genetic Algorithm Training")
ylabel("Fitness")
xlabel("Generation")
legend("Best Fitness in Each Generation", "Average Fitness in Each Generation", "Worst Fitness in Each Generation")
hold off