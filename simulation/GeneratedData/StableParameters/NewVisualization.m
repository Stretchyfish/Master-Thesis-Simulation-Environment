% WIth help from https://towardsdatascience.com/the-art-of-effective-visualization-of-multi-dimensional-data-6c7202990c57
% https://se.mathworks.com/matlabcentral/answers/373286-plot-multiple-variables-in-different-colors-with-scatter3

stable_hex = importdata("StableParameters_flat_Update_Hexapod_new.csv");

hex_vstep = [];
hex_hstep = [];
hex_speed = [];

hex_stability_percentage = [];
hex_average_stability_margin = [];
hex_completed_task = [];

for i = 1:6:length(stable_hex)

    hex_vstep(end + 1) = stable_hex(i);
    hex_hstep(end + 1) = stable_hex(i + 1);
    hex_speed(end + 1) = stable_hex(i + 2);
    hex_stability_percentage(end + 1) = stable_hex(i + 3);
    hex_average_stability_margin(end + 1) = stable_hex(i + 4);
    hex_completed_task(end + 1) = stable_hex(i + 5);
end

figure(1)

subplot(1,3,1);
hold on;
title("Hexapod Gait Stability Percentage")

%markerColors = [];

%hsv(hex_stability_percentage(i))
%for i=1:1:length(hex_stability_percentage)
%    markerColors(end + 1) = hsv(hex_stability_percentage(i));

%end

%markerColors = hsv(length(hex_stability_percentage));
%markerColor = interp1([0;1],[1 0 0; 0 1 0],hex_stability_percentage); % https://se.mathworks.com/matlabcentral/answers/616663-color-map-from-green-to-red
bottom_color = [1 0 0]; % red
top_color = [0 1 0]; % green
n_colors = 64; % number of colors in the map
custom_map = [linspace(bottom_color(1), top_color(1), n_colors)', ...
              linspace(bottom_color(2), top_color(2), n_colors)', ...
              linspace(bottom_color(3), top_color(3), n_colors)']; % Chat GPT


markerColor = hex_stability_percentage * 100;
s = scatter3(hex_vstep, hex_hstep, hex_speed,50, markerColor, 'filled');
s.AlphaData = 0.1;
xlabel("Vertical Step Size [degree]");
ylabel("Horizontal Step Size [degree]");
zlabel("Speed [Hz]");
grid on

cb = colorbar;
colormap(custom_map)
ylabel(cb,"Stability Percentage [%]");
caxis([0, 100])
%colormap "RGB"
%colormap(markerColor)

%colormap(markerColor,"winter")
%s = scatter3(hex_vstep, hex_hstep, hex_speed,'filled', hex_stability_percentage ,'ColorVariable','Diastolic');

%plot3(mid_vstep, mid_hstep, mid_speed, 'o', 'Color', 'y');
%plot3(bad_vstep, bad_hstep, bad_speed, 'o', 'Color', 'r');

%legend("Stable and fast", "Stable and slow","unstable");
hold off;

subplot(1,3,2)
%title("Hexapod Gait Task Completion")


%hsv(hex_stability_percentage(i))

markerColors = interp1([0;1],[1 0 0; 0 1 0], hex_completed_task);

%markerColors = hsv(length(hex_stability_percentage));
%markerColors = hex_completed_task;

s = scatter3(hex_vstep, hex_hstep, hex_speed,50, markerColors, 'filled');
s.AlphaData = 0.1;
xlabel("Vertical Step Size [degree]");
ylabel("Horizontal Step Size [degree]");
zlabel("Speed [Hz]");
grid on

%s = scatter3(hex_vstep, hex_hstep, hex_speed,'filled', hex_stability_percentage ,'ColorVariable','Diastolic');
%colorbar(makerColors)
%plot3(mid_vstep, mid_hstep, mid_speed, 'o', 'Color', 'y');
%plot3(bad_vstep, bad_hstep, bad_speed, 'o', 'Color', 'r');

%legend("Stable and fast", "Stable and slow","unstable");
title("Hexapod Gait Task Completion")
hold off;

subplot(1,3,3);
hold on;
title("Hexapod Gait Stability Margin")

%markerColors = [];

%hsv(hex_stability_percentage(i))
%for i=1:1:length(hex_stability_percentage)
%    markerColors(end + 1) = hsv(hex_stability_percentage(i));

%end

%markerColors = hsv(length(hex_stability_percentage));
%markerColor = interp1([0;1],[1 0 0; 0 1 0],hex_stability_percentage); % https://se.mathworks.com/matlabcentral/answers/616663-color-map-from-green-to-red
bottom_color = [1 0 0]; % red
top_color = [0 1 0]; % green
n_colors = 64; % number of colors in the map
custom_map = [linspace(bottom_color(1), top_color(1), n_colors)', ...
              linspace(bottom_color(2), top_color(2), n_colors)', ...
              linspace(bottom_color(3), top_color(3), n_colors)']; % Chat GPT


markerColors = hex_average_stability_margin;
s = scatter3(hex_vstep, hex_hstep, hex_speed, 50, markerColor, 'filled');
s.AlphaData = 0.1;
xlabel("Vertical Step Size [degree]");
ylabel("Horizontal Step Size [degree]");
zlabel("Speed [Hz]");
grid on
cb = colorbar;
colormap(custom_map)
ylabel(cb,"Stability Margin [m]");

caxis([-0.4583, 1.0955])

%colormap "RGB"
%colormap(markerColor)

%colormap(markerColor,"winter")
%s = scatter3(hex_vstep, hex_hstep, hex_speed,'filled', hex_stability_percentage ,'ColorVariable','Diastolic');

%plot3(mid_vstep, mid_hstep, mid_speed, 'o', 'Color', 'y');
%plot3(bad_vstep, bad_hstep, bad_speed, 'o', 'Color', 'r');

%legend("Stable and fast", "Stable and slow","unstable");
hold off;

%******************** QUADRUPED********************************


stable_quad = importdata("StableParameters_flat_Update_Quadruped_3.csv");

quad_vstep = [];
quad_hstep = [];
quad_speed = [];

quad_stability_percentage = [];
quad_average_stability_margin = [];
quad_completed_task = [];

for i = 1:6:length(stable_quad)

    quad_vstep(end + 1) = stable_quad(i);
    quad_hstep(end + 1) = stable_quad(i + 1);
    quad_speed(end + 1) = stable_quad(i + 2);
    quad_stability_percentage(end + 1) = stable_quad(i + 3);
    quad_average_stability_margin(end + 1) = stable_quad(i + 4);
    quad_completed_task(end + 1) = stable_quad(i + 5);
end

figure(2)

subplot(1,3,1);
hold on;
title("Quadruped Gait Stability Percentage")

%markerColors = [];

%hsv(hex_stability_percentage(i))
%for i=1:1:length(hex_stability_percentage)
%    markerColors(end + 1) = hsv(hex_stability_percentage(i));

%end

%markerColors = hsv(length(hex_stability_percentage));
%markerColor = interp1([0;1],[1 0 0; 0 1 0],hex_stability_percentage); % https://se.mathworks.com/matlabcentral/answers/616663-color-map-from-green-to-red
bottom_color = [1 0 0]; % red
top_color = [0 1 0]; % green
n_colors = 64; % number of colors in the map
custom_map = [linspace(bottom_color(1), top_color(1), n_colors)', ...
              linspace(bottom_color(2), top_color(2), n_colors)', ...
              linspace(bottom_color(3), top_color(3), n_colors)']; % Chat GPT


markerColor = quad_stability_percentage * 100;
s = scatter3(quad_vstep, quad_hstep, quad_speed,50, markerColor, 'filled', 'MarkerFaceAlpha', 1);
s.AlphaData = 0.1;
xlabel("Vertical Step Size [degree]");
ylabel("Horizontal Step Size [degree]");
zlabel("Speed [Hz]");
grid on

cb = colorbar;
colormap(custom_map)
ylabel(cb,"Stability Percentage [%]");
caxis([0, 100])



%colormap "RGB"
%colormap(markerColor)

%colormap(markerColor,"winter")
%s = scatter3(hex_vstep, hex_hstep, hex_speed,'filled', hex_stability_percentage ,'ColorVariable','Diastolic');

%plot3(mid_vstep, mid_hstep, mid_speed, 'o', 'Color', 'y');
%plot3(bad_vstep, bad_hstep, bad_speed, 'o', 'Color', 'r');

%legend("Stable and fast", "Stable and slow","unstable");
hold off;

subplot(1,3,2)
%title("Hexapod Gait Task Completion")


%hsv(hex_stability_percentage(i))

markerColors = interp1([0;1],[1 0 0; 0 1 0], quad_completed_task);

%markerColors = hsv(length(hex_stability_percentage));
%markerColors = hex_completed_task;

s = scatter3(quad_vstep, quad_hstep, quad_speed,50, markerColors, 'filled');
s.AlphaData = 0.1;
xlabel("Vertical Step Size [degree]");
ylabel("Horizontal Step Size [degree]");
zlabel("Speed [Hz]");
grid on

%s = scatter3(hex_vstep, hex_hstep, hex_speed,'filled', hex_stability_percentage ,'ColorVariable','Diastolic');
%colorbar(makerColors)
%plot3(mid_vstep, mid_hstep, mid_speed, 'o', 'Color', 'y');
%plot3(bad_vstep, bad_hstep, bad_speed, 'o', 'Color', 'r');

%legend("Stable and fast", "Stable and slow","unstable");
title("Quadruped Gait Task Completion")
hold off;

subplot(1,3,3);
hold on;
title("Quadruped Gait Stability Margin")

%markerColors = [];

%hsv(hex_stability_percentage(i))
%for i=1:1:length(hex_stability_percentage)
%    markerColors(end + 1) = hsv(hex_stability_percentage(i));

%end

%markerColors = hsv(length(hex_stability_percentage));
%markerColor = interp1([0;1],[1 0 0; 0 1 0],hex_stability_percentage); % https://se.mathworks.com/matlabcentral/answers/616663-color-map-from-green-to-red
bottom_color = [1 0 0]; % red
top_color = [0 1 0]; % green
n_colors = 64; % number of colors in the map
custom_map = [linspace(bottom_color(1), top_color(1), n_colors)', ...
              linspace(bottom_color(2), top_color(2), n_colors)', ...
              linspace(bottom_color(3), top_color(3), n_colors)']; % Chat GPT


markerColors = quad_average_stability_margin;
s = scatter3(quad_vstep, quad_hstep, quad_speed, 50, markerColors, 'filled');
s.AlphaData = 0.1;
xlabel("Vertical Step Size [degree]");
ylabel("Horizontal Step Size [degree]");
zlabel("Speed [Hz]");
grid on
cb = colorbar;
colormap(custom_map)
ylabel(cb,"Stability Margin [m]");

caxis([-0.366, 1.112])

%colormap "RGB"
%colormap(markerColor)

%colormap(markerColor,"winter")
%s = scatter3(hex_vstep, hex_hstep, hex_speed,'filled', hex_stability_percentage ,'ColorVariable','Diastolic');

%plot3(mid_vstep, mid_hstep, mid_speed, 'o', 'Color', 'y');
%plot3(bad_vstep, bad_hstep, bad_speed, 'o', 'Color', 'r');

%legend("Stable and fast", "Stable and slow","unstable");
hold off;

