hex_avr_fitness = importdata("AvrFitness_destruction.csv");
hex_worst_fitness = importdata("WorstFitness_destruction.csv");
hex_best_fitness = importdata("BestFitness_destruction.csv");

figure(1)

axis square
hold on
plot(hex_best_fitness, Color='green')
plot(hex_avr_fitness, Color='blue')
plot(hex_worst_fitness, Color='red')
title("Parameter Estimation Hexapod getting damaged - Genetic Algorithm Training")
ylabel("Fitness")
xlabel("Generation")
legend("Best Fitness in Each Generation", "Average Fitness in Each Generation", "Worst Fitness in Each Generation")
hold off

