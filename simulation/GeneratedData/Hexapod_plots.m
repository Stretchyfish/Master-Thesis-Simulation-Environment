%****************** Legs on ground ******************************

leg1 = importdata("leg1_on_ground.csv");
leg2 = importdata("leg2_on_ground.csv");
leg3 = importdata("leg3_on_ground.csv");
leg4 = importdata("leg4_on_ground.csv");
leg5 = importdata("leg5_on_ground.csv");
leg6 = importdata("leg6_on_ground.csv");

figure(1)
hold on;

plot(leg1, '--');
plot(leg2, '--');
plot(leg3, '--');
plot(leg4, '--');
plot(leg5, '--');
plot(leg6, '--');

hold off;

title("Leg ground contact")
legend(["leg1", "leg2", "leg3", "leg4", "leg5", "leg6"])
xlim([0 200]);
ylim([-0.5, 1.5]);


%****************** Legs positions ******************************

leg1_position_x = importdata("leg1_position_x.csv");
leg1_position_y = importdata("leg1_position_y.csv");
leg1_position_z = importdata("leg1_position_z.csv");

leg2_position_x = importdata("leg2_position_x.csv");
leg2_position_y = importdata("leg2_position_y.csv");
leg2_position_z = importdata("leg2_position_z.csv");

leg3_position_x = importdata("leg3_position_x.csv");
leg3_position_y = importdata("leg3_position_y.csv");
leg3_position_z = importdata("leg3_position_z.csv");

leg4_position_x = importdata("leg4_position_x.csv");
leg4_position_y = importdata("leg4_position_y.csv");
leg4_position_z = importdata("leg4_position_z.csv");

leg5_position_x = importdata("leg5_position_x.csv");
leg5_position_y = importdata("leg5_position_y.csv");
leg5_position_z = importdata("leg5_position_z.csv");

leg6_position_x = importdata("leg6_position_x.csv");
leg6_position_y = importdata("leg6_position_y.csv");
leg6_position_z = importdata("leg6_position_z.csv");

figure(2)
hold on;

plot(leg1_position_y);
plot(leg2_position_y);
%plot(leg3_position_y);
%plot(leg4_position_y);
%plot(leg5_position_y);
%plot(leg6_position_y);

hold off;
title("Leg position")
legend(["leg1", "leg2", "leg3", "leg4", "leg5", "leg6"])

%************* Robot position and velocity **********************

robot_position_x = importdata("position_x.csv");
robot_position_y = importdata("position_y.csv");
robot_position_z = importdata("position_z.csv");

robot_CoM_position_x = importdata("Hex_center_of_mass_x.csv");
robot_CoM_position_y = importdata("Hex_center_of_mass_y.csv");
robot_CoM_position_z = importdata("Hex_center_of_mass_z.csv");

figure(3)
hold on;

%plot3(robot_position_x, robot_position_z,  robot_position_y); % The y and z axis has been turned.
plot3(robot_CoM_position_x, robot_CoM_position_z, robot_CoM_position_y); % The y and z axis has been turned.
plot3(leg1_position_x, leg1_position_z, leg1_position_y);
plot3(leg2_position_x, leg2_position_z, leg2_position_y);
plot3(leg3_position_x, leg3_position_z, leg3_position_y);
plot3(leg4_position_x, leg4_position_z, leg4_position_y);
plot3(leg5_position_x, leg5_position_z, leg5_position_y);
plot3(leg6_position_x, leg6_position_z, leg6_position_y);

stability_points = [];

for i = 1:1:length(leg1)
    
    if (leg1(i) == 1) && (leg3(i) == 1) && (leg5(i) == 1)

        P1 = [leg1_position_x(i); leg3_position_x(i); leg5_position_x(i)];
        P2 = [leg1_position_z(i); leg3_position_z(i); leg5_position_z(i)];
        P3 = [leg1_position_y(i); leg3_position_y(i); leg5_position_y(i)];
        p = fill3(P1, P2, P3, [0.8500 0.3250 0.0980], 'LineStyle','none');
        p(1).FaceAlpha = 0.2;

        if inpolygon(robot_CoM_position_x(i), robot_CoM_position_z(i), P1, P2) == 1
            scatter3(robot_CoM_position_x(i), robot_CoM_position_z(i), 0, 'g');
        end

    elseif (leg2(i) == 1) && (leg4(i) == 1) && (leg6(i) == 1)

        P1 = [leg2_position_x(i); leg4_position_x(i); leg6_position_x(i)];
        P2 = [leg2_position_z(i); leg4_position_z(i); leg6_position_z(i)];
        P3 = [leg2_position_y(i); leg4_position_y(i); leg6_position_y(i)];
        p = fill3(P1, P2, P3, [0.8500 0.3250 0.0980], 'LineStyle','none');
        p(1).FaceAlpha = 0.2;

        if inpolygon(robot_CoM_position_x(i), robot_CoM_position_z(i), P1, P2) == 1
            scatter3(robot_CoM_position_x(i), robot_CoM_position_z(i), 0, 'g');
        end
    else
        scatter3(robot_CoM_position_x(i), robot_CoM_position_z(i), 0, 'r');
    end
    
    
    if leg1(i) == 1
        %scatter3(robot_CoM_position_x(i), robot_CoM_position_z(i), robot_CoM_position_y(i), 'g');
        %disp(leg1(i)) 
    else
        %scatter3(robot_CoM_position_x(i), robot_CoM_position_z(i), robot_CoM_position_y(i), 'r');
    end
end

%fill3()

legend(["Robot CoM", "leg1", "leg2", "leg3", "leg4", "leg5", "leg6"])
xlabel("x")
ylabel("y")
zlabel("z")

%xlim([0,2])
%ylim([0,2])
%zlim([0,2])

hold off;
title("Robot position")

