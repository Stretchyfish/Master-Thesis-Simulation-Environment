hex_avr_fitness = importdata("AvrFitness.csv");
hex_worst_fitness = importdata("WorstFitness.csv");
hex_best_fitness = importdata("BestFitness.csv");

%quad_avr_fitness = importdata("AvrFitness_small_incline_quadruped.csv");
%quad_worst_fitness = importdata("WorstFitness_small_incline_quadruped.csv");
%quad_best_fitness = importdata("BestFitness_small_incline_quadruped.csv");

figure(1)

subplot(1,2,1)

axis square
hold on
plot(hex_worst_fitness, Color='red')
plot(hex_avr_fitness, Color='blue')
plot(hex_best_fitness, Color='green')
title("Parameter Estimator (Hexapod) - Genetic Algorithm Training")
ylabel("Fitness")
xlabel("Generation")
legend("Best Fitness in Each Generation", "Average Fitness in Each Generation", "Worst Fitness in Each Generation")
hold off

%subplot(1,2,2)

%axis square
%hold on
%plot(quad_worst_fitness, Color='red')
%plot(quad_avr_fitness, Color='blue')
%plot(quad_best_fitness, Color='green')
%title("Parameter Estimator (Quadruped) - Genetic Algorithm Training")
%ylabel("Fitness")
%xlabel("Generation")
%legend("Best Fitness in Each Generation", "Average Fitness in Each Generation", "Worst Fitness in Each Generation")
%hold off