
%impData = importdata("Networksmall_incline_terrain_quad_agent_last2Exploration.csv");
impData = importdata("NetworkTryingSomething3Exploration4.csv");


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


ver_data = ver_data(1:1:20);
hor_data = hor_data(1:1:20);
speed_data = speed_data(1:1:20);
height_data = height_data(1:1:20);

for i = 1:1:length(ver_data)
    ver_data(i) = RangeMapping(ver_data(i), 0, 1, 0, 45);
    hor_data(i) = RangeMapping(hor_data(i), 0, 1, 0, 45);
    speed_data(i) = RangeMapping(speed_data(i), 0, 1, 0.1, 4);
    height_data(i) = RangeMapping(height_data(i), 0, 1, 0, 40);
end

x = linspace(0, 10, 20);

figure(1)

hold on

subplot(1,4,1);
axis square
plot(x, ver_data);
title("Vertical Step Size")
ylabel("Degree")
xlabel("Terrain Steepness degree")
ylim([0, 45])

subplot(1,4,2);
axis square
plot(x, hor_data);
title("Horizontal Step Size")
ylabel("Degree")
xlabel("Terrain Steepness degree")
ylim([0, 45])

subplot(1,4,3);
axis square
plot(x, speed_data);
title("Speed")
ylabel("Hz");
xlabel("Terrain Steepness degree")
ylim([0.1, 4])

subplot(1,4,4);
axis square
plot(x, height_data);
title("Height")
ylabel("Degree");
xlabel("Terrain Steepness degree")
ylim([0, 40])

sgtitle("Hexapod approximated functions of difficult terrain with small inclines")
hold off