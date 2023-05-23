%****************** Legs on ground ******************************

state_Hex = importdata("state_Hex.csv");
state_Quad = importdata("state_Quad.csv");
Horizontal_Step_Hex = importdata("Horizontal_Step_Hex.csv");
Horizontal_Step_Quad = importdata("Horizontal_Step_Quad.csv");
Vertical_Step_Hex = importdata("Vertical_Step_Hex.csv");
Vertical_Step_Quad = importdata("Vertical_Step_Quad.csv");
Speed_Hex = importdata("Speed_Hex.csv");
Speed_Quad = importdata("Speed_Quad.csv");

Targets_Reached_Hex = importdata("TargetsReachedHex.csv");
Targets_Reached_Quad = importdata("TargetsReachedQuad.csv");

if state_Hex == Speed_Hex
    disp("Is the same");
end

HexTargetsReachedTime = [];
lastValueHex = Targets_Reached_Hex(1);
for t = 1:1:length(Targets_Reached_Hex)
    
    if Targets_Reached_Hex(t) ~= lastValueHex
        HexTargetsReachedTime(end + 1) = t;
        lastValueHex = Targets_Reached_Hex(t);
    end
end

QuadTargetsReachedTime = [];
lastValueQuad = Targets_Reached_Quad(1);
for t = 1:1:length(Targets_Reached_Quad)
    
    if Targets_Reached_Quad(t) ~= lastValueQuad;
        QuadTargetsReachedTime(end + 1) = t;
        lastValueQuad = Targets_Reached_Quad(t);
    end
end


figure(1)
hold on;

% HEXAPOD %

subplot(2, 4, 1);
plot(state_Hex, '--');
title("State");
ylabel("Hexapod");

for i = 1:1:length(HexTargetsReachedTime)
    xline(HexTargetsReachedTime(i),'--r');
end


subplot(2, 4, 2);
plot(Horizontal_Step_Hex, '--');
title("Vertical Step size");

for i = 1:1:length(HexTargetsReachedTime)
    xline(HexTargetsReachedTime(i),'--r');
end

subplot(2, 4, 3);
plot(Vertical_Step_Hex, '--');
title("Horizontal Step Size");

for i = 1:1:length(HexTargetsReachedTime)
    xline(HexTargetsReachedTime(i),'--r');
end

subplot(2, 4, 4);
plot(Speed_Hex, '--');
title("Speed");

for i = 1:1:length(HexTargetsReachedTime)
    xline(HexTargetsReachedTime(i),'--r');
end

% Quadruped %

subplot(2, 4, 5);
plot(state_Quad, '--');
ylabel("Quadruped");

for i = 1:1:length(QuadTargetsReachedTime)
    xline(QuadTargetsReachedTime(i),'--r');
end

subplot(2, 4, 6);
plot(Horizontal_Step_Quad, '--');

for i = 1:1:length(QuadTargetsReachedTime)
    xline(QuadTargetsReachedTime(i),'--r');
end

subplot(2, 4, 7);
plot(Vertical_Step_Quad, '--');

for i = 1:1:length(QuadTargetsReachedTime)
    xline(QuadTargetsReachedTime(i),'--r');
end

subplot(2, 4, 8);
plot(Speed_Quad, '--');

for i = 1:1:length(QuadTargetsReachedTime)
    xline(QuadTargetsReachedTime(i),'--r');
end

hold off;

%xlim([0 200]);
%ylim([-0.5, 1.5]);