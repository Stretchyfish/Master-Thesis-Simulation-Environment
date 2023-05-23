function [outputState] = plotdata(fileName)

%data = csvread(fileName, "Delimiter");

data = importdata(fileName);

plot(data);

outputState = true;
end

