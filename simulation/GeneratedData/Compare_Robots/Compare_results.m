%****************** Legs on ground ******************************

hexapod_outputs = importdata("Generic_Hexapod.csv");
quadruped_outputs = importdata("Generic_Quadruped.csv");

hex_vstep = [];
hex_hstep = [];
hex_speed = [];
hex_underleg = [];
hex_targets_reached = [];

quad_vstep = [];
quad_hstep = [];
quad_speed = [];
quad_underleg = [];
quad_targets_reached = [];

disp(length(hexapod_outputs))

i = 1;
while 1
    input1 = hexapod_outputs(i);
    input2 = hexapod_outputs(i + 1);
    input3 = hexapod_outputs(i + 2);
    
    hex_vstep(end + 1) = hexapod_outputs(i + 3);
    hex_hstep(end + 1) = hexapod_outputs(i + 4);
    hex_speed(end + 1) = hexapod_outputs(i + 5);
    hex_underleg(end + 1) = hexapod_outputs(i + 6);
    hex_targets_reached(end + 1) = hexapod_outputs(i + 7);

    i = i + 8;

    if i >= length(hexapod_outputs) - 2
        break
    end
end

i = 1;
while 1
    input1 = quadruped_outputs(i);
    input2 = quadruped_outputs(i + 1);
    input3 = quadruped_outputs(i + 2);
    
    quad_vstep(end + 1) = quadruped_outputs(i + 3);
    quad_hstep(end + 1) = quadruped_outputs(i + 4);
    quad_speed(end + 1) = quadruped_outputs(i + 5);
    quad_underleg(end + 1) = quadruped_outputs(i + 6);
    quad_targets_reached(end + 1) = quadruped_outputs(i + 7);

    i = i + 8;

    if i >= length(quadruped_outputs) - 3
        break;
    end
end

disp("got here")

HexTargetsReachedTime = [];
lastValueHex = hex_targets_reached(1);
for t = 1:1:length(hex_targets_reached)
    
    if hex_targets_reached(t) ~= lastValueHex
        HexTargetsReachedTime(end + 1) = t;
        lastValueHex = hex_targets_reached(t);
    end
end

QuadTargetsReachedTime = [];
lastValueQuad = quad_targets_reached(1);
for t = 1:1:length(quad_targets_reached)
    
    if quad_targets_reached(t) ~= lastValueQuad
        QuadTargetsReachedTime(end + 1) = t;
        lastValueQuad = quad_targets_reached(t);
    end
end


figure(1)
hold on;

% HEXAPOD %

subplot(2, 4, 1);
plot(hex_vstep, '--');
title("vstep");
ylabel("Hexapod");

for i = 1:1:length(HexTargetsReachedTime)
    xline(HexTargetsReachedTime(i),'--r');
end


subplot(2, 4, 2);
plot(hex_hstep, '--');
title("hstep");

for i = 1:1:length(HexTargetsReachedTime)
    xline(HexTargetsReachedTime(i),'--r');
end

subplot(2, 4, 3);
plot(hex_speed, '--');
title("speed");

for i = 1:1:length(HexTargetsReachedTime)
    xline(HexTargetsReachedTime(i),'--r');
end

subplot(2, 4, 4);
plot(hex_underleg, '--');
title("underleg");

for i = 1:1:length(HexTargetsReachedTime)
    xline(HexTargetsReachedTime(i),'--r');
end

% Quadruped %

subplot(2, 4, 5);
plot(quad_vstep, '--');
ylabel("Quadruped");

for i = 1:1:length(QuadTargetsReachedTime)
    xline(QuadTargetsReachedTime(i),'--r');
end

subplot(2, 4, 6);
plot(quad_hstep, '--');

for i = 1:1:length(QuadTargetsReachedTime)
    xline(QuadTargetsReachedTime(i),'--r');
end

subplot(2, 4, 7);
plot(quad_speed, '--');

for i = 1:1:length(QuadTargetsReachedTime)
    xline(QuadTargetsReachedTime(i),'--r');
end

subplot(2, 4, 8);
plot(quad_underleg, '--');

for i = 1:1:length(QuadTargetsReachedTime)
    xline(QuadTargetsReachedTime(i),'--r');
end

hold off;

%xlim([0 200]);
%ylim([-0.5, 1.5]);