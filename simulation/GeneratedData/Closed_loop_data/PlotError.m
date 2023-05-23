

errorX = importdata("CoMErrorX.csv");
errorY = importdata("CoMErrorY.csv");
errorZ = importdata("CoMErrorZ.csv");

subplot(3,1,1);
plot(errorX);
title("CoM Error X")
xlabel("Time steps")
ylabel("Error")

subplot(3,1,2);
plot(errorZ);
title("CoM Error Y")
xlabel("Time steps")
ylabel("Error")

subplot(3,1,3);
plot(errorY);
title("CoM Error Z")
xlabel("Time steps")
ylabel("Error")

