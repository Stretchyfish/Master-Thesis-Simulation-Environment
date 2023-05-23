

impData = importdata("Networklast_destruction_test_lastExploration.csv");


data = [];

ver_data = [];
hor_data = [];
speed_data = [];
height_data = [];

i = 1;
%i = length(impData) / 2 + 1;
while true 

    if i > length(impData) / 2 % Quadruped
        break
    end

    %if i > length(impData) - 1% QHexapod
    %    break
    %end

    ver_data(end + 1) = impData(i + 2);
    hor_data(end + 1) = impData(i + 3);
    speed_data(end + 1) = impData(i + 4);
    height_data(end + 1) = impData(i + 5);

    i = i + 6;
end


%ver_data = ver_data(1:1:20);
%hor_data = hor_data(1:1:20);
%speed_data = speed_data(1:1:20);
%height_data = height_data(1:1:20);

x = linspace(-10, 10, 41);

figure(1)

hold on

subplot(1,4,1);
axis square
plot(x, ver_data);
title("Vertical Step Size")
xlabel("Terrain Steepness")
ylim([0, 1])

subplot(1,4,2);
axis square
plot(x, hor_data);
title("Horizontal Step Size")
xlabel("Terrain Steepness")
ylim([0, 1])

subplot(1,4,3);
axis square
plot(x, speed_data);
title("Speed")
xlabel("Terrain Steepness")
ylim([0, 1])

subplot(1,4,4);
axis square
plot(x, height_data);
title("Height")
xlabel("Terrain Steepness")
ylim([0, 1])

sgtitle("Quadruped approximated functions of difficult terrain with small inclines")
hold off