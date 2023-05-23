function output = RangeMapping(value, o_min, o_max, n_min, n_max) % https://prime31.github.io/simple-value-mapping/
    output = n_min + (value - o_min) * (n_max - n_min) / (o_max - o_min)
end
