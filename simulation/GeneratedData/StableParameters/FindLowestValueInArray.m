stable_hex = importdata("StableParameters_flat_Update_Hexapod.csv");

hex_vstep = [];
hex_hstep = [];
hex_speed = [];

hex_stability_percentage = [];
hex_average_stability_margin = [];
hex_completed_task = [];

lowest_margin_observed = 1000000;
highest_margin_observed = -1000000;

for i = 1:6:length(stable_hex)

    hex_vstep(end + 1) = stable_hex(i);
    hex_hstep(end + 1) = stable_hex(i + 1);
    hex_speed(end + 1) = stable_hex(i + 2);
    hex_stability_percentage(end + 1) = stable_hex(i + 3);
    hex_average_stability_margin(end + 1) = stable_hex(i + 4);
    hex_completed_task(end + 1) = stable_hex(i + 5);

    if(stable_hex(i + 4) > highest_margin_observed)
        highest_margin_observed = stable_hex(i + 4);
    end

    if(stable_hex(i + 4) < lowest_margin_observed)
        lowest_margin_observed = stable_hex(i + 4);
    end
end

disp("Hexapod")
disp(highest_margin_observed)
disp(lowest_margin_observed)

%************ QUADRUPED *****************

stable_quad = importdata("StableParameters_flat_Update_Quadruped.csv");

quad_vstep = [];
quad_hstep = [];
quad_speed = [];

quad_stability_percentage = [];
quad_average_stability_margin = [];
quad_completed_task = [];

lowest_margin_observed = 1000000;
highest_margin_observed = -1000000;

for i = 1:6:length(stable_quad)

    quad_vstep(end + 1) = stable_quad(i);
    quad_hstep(end + 1) = stable_quad(i + 1);
    quad_speed(end + 1) = stable_quad(i + 2);
    quad_stability_percentage(end + 1) = stable_quad(i + 3);
    quad_average_stability_margin(end + 1) = stable_quad(i + 4);
    quad_completed_task(end + 1) = stable_quad(i + 5);

    if(stable_quad(i + 4) > highest_margin_observed)
        highest_margin_observed = stable_quad(i + 4);
    end

    if(stable_quad(i + 4) < lowest_margin_observed)
        lowest_margin_observed = stable_quad(i + 4);
    end
end

disp("Quadruped")
disp(highest_margin_observed)
disp(lowest_margin_observed)
