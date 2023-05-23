function [o1, o2, o3, o4] = MoreRange(val1, val2, val3, val4)
    o1 = RangeMapping(val1, 0, 1, 0, 45);
    o2 = RangeMapping(val2, 0, 1, 0, 45);
    o3 = RangeMapping(val3, 0, 1, 0.1, 4);
    o4 = RangeMapping(val4, 0, 1, 0, 40);
end