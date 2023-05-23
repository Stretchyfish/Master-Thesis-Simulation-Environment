
stable_parameters = importdata("StableParameters_flat_Update_Hexapod.csv");

good_vstep = [];
good_hstep = [];
good_speed = [];

mid_vstep = [];
mid_hstep = [];
mid_speed = [];

bad_vstep = [];
bad_hstep = [];
bad_speed = [];

status = [];
finish_status = []; % Remember to implement this in the next one!

for i = 1:5:length(stable_parameters)
    status(end + 1) = stable_parameters(i + 3);
    finish_status(end + 1) = stable_parameters(i + 4);

    if status(end) == 1 && finish_status(end) == 1
        good_vstep(end + 1) = stable_parameters(i);
        good_hstep(end + 1) = stable_parameters(i + 1);
        good_speed(end + 1) = stable_parameters(i + 2);

    elseif status(end) == 1 && finish_status(end) ~= 1

        mid_vstep(end + 1) = stable_parameters(i);
        mid_hstep(end + 1) = stable_parameters(i + 1);
        mid_speed(end + 1) = stable_parameters(i + 2);

    else
        bad_vstep(end + 1) = stable_parameters(i);
        bad_hstep(end + 1) = stable_parameters(i + 1);
        bad_speed(end + 1) = stable_parameters(i + 2);
    end
end


Quad_stable_parameters = importdata("Quad_StableParameters.csv");

good_vstep_q = [];
good_hstep_q = [];
good_speed_q = [];

mid_vstep_q = [];
mid_hstep_q = [];
mid_speed_q = [];

bad_vstep_q = [];
bad_hstep_q = [];
bad_speed_q = [];

status_q = [];
finish_status_q = []; % Remember to implement this in the next one!

for i = 1:5:length(Quad_stable_parameters)
    status_q(end + 1) = Quad_stable_parameters(i + 3);
    finish_status_q(end + 1) = Quad_stable_parameters(i + 4);

    if status_q(end) == 1 && finish_status_q(end) == 1
        good_vstep_q(end + 1) = Quad_stable_parameters(i);
        good_hstep_q(end + 1) = Quad_stable_parameters(i + 1);
        good_speed_q(end + 1) = Quad_stable_parameters(i + 2);

    elseif status_q(end) == 1 && finish_status_q(end) ~= 1

        mid_vstep_q(end + 1) = Quad_stable_parameters(i);
        mid_hstep_q(end + 1) = Quad_stable_parameters(i + 1);
        mid_speed_q(end + 1) = Quad_stable_parameters(i + 2);

    else
        bad_vstep_q(end + 1) = Quad_stable_parameters(i);
        bad_hstep_q(end + 1) = Quad_stable_parameters(i + 1);
        bad_speed_q(end + 1) = Quad_stable_parameters(i + 2);
    end
end


figure(1)

subplot(1,2,1);
hold on;
title("Hexapod Gait Based Control Parameters Stability")
plot3(good_vstep, good_hstep, good_speed, 'o', 'Color', 'g');
plot3(mid_vstep, mid_hstep, mid_speed, 'o', 'Color', 'y');
plot3(bad_vstep, bad_hstep, bad_speed, 'o', 'Color', 'r');
xlabel("Vertical Step Size [degree]");
ylabel("Horizontal Step Size [degree]");
zlabel("Speed");
legend("Stable and fast", "Stable and slow","unstable");
hold off;

subplot(1,2,2);
hold on;
title("Quadruped Gait Based Control Parameters Stability")
plot3(good_vstep_q, good_hstep_q, good_speed_q, 'o', 'Color', 'g');
plot3(mid_vstep_q, mid_hstep_q, mid_speed_q, 'o', 'Color', 'y');
plot3(bad_vstep_q, bad_hstep_q, bad_speed_q, 'o', 'Color', 'r');
xlabel("Vertical Step Size [degree]");
ylabel("Horizontal Step Size [degree]");
zlabel("Speed");
legend("Stable and fast", "Stable and slow","unstable");
hold off;

