
L
vector_observationPlaceholder*
shape:���������*
dtype0
?
is_continuous_controlConst*
value	B :*
dtype0
8
version_numberConst*
dtype0*
value	B :
5
memory_sizeConst*
dtype0*
value	B : 
=
action_output_shapeConst*
value	B :*
dtype0
�
#policy/main_graph_0/hidden_0/kernelConst*
dtype0*�
value�B�
"��g?>W�>+�.?�Q9>�����:���	?\i�>��6=���?�@>��H>�R�>�?9��>Uɼ�>!�d"o�_�?�W,?��>��>Y�p>U����1�L(>((B>@�?��>�N��FN`�H�=�j>��ݾ��=�!=��Z�?�� `H�qѽ�%M>�@/>��m�Cy�=&�>�
?:�o�9�r�Y>l >�?�G�E<�>� ��f���便J/>���>�$n��#�>
�
(policy/main_graph_0/hidden_0/kernel/readIdentity#policy/main_graph_0/hidden_0/kernel*
T0*6
_class,
*(loc:@policy/main_graph_0/hidden_0/kernel
v
!policy/main_graph_0/hidden_0/biasConst*=
value4B2
"(k��U��>���=��g>W̰=w#�=��=���Ռ>��w>*
dtype0
�
&policy/main_graph_0/hidden_0/bias/readIdentity!policy/main_graph_0/hidden_0/bias*
T0*4
_class*
(&loc:@policy/main_graph_0/hidden_0/bias
�
#policy/main_graph_0/hidden_0/MatMulMatMulvector_observation(policy/main_graph_0/hidden_0/kernel/read*
transpose_a( *
T0*
transpose_b( 
�
$policy/main_graph_0/hidden_0/BiasAddBiasAdd#policy/main_graph_0/hidden_0/MatMul&policy/main_graph_0/hidden_0/bias/read*
T0*
data_formatNHWC
^
$policy/main_graph_0/hidden_0/SigmoidSigmoid$policy/main_graph_0/hidden_0/BiasAdd*
T0
|
 policy/main_graph_0/hidden_0/MulMul$policy/main_graph_0/hidden_0/BiasAdd$policy/main_graph_0/hidden_0/Sigmoid*
T0
�
#policy/main_graph_0/hidden_1/kernelConst*�
value�B�

"�
TF>��#�$�Z�]>�q>ւQ��俾�A1��u�=L6?��j=�S�<[��=(��>�~=�)�=e�?gl�>_�?�+��A ?��g���n��X��B'?l��>p�c��>�y����
m�;^4b<���>��n;��>�M?S:�>s�?/[m�k
�>XW��?�	>�B�=�j��]��>
����m�>��(?�d;���a��>�©��ѽ�K����7�=g��@?�O�>ML�Chy���˾蓯>I��M~l��w,�f&�=�|�>[�󾻷6���#>�'�>���p�X�]q�<�?\��T˾J>���=m���8DM>F >�&�h>�?G{�&|:?p?�>�q�>8��>�/���v?��#��K��C�;3X>_A�>5�;?sW���O��/�*
dtype0
�
(policy/main_graph_0/hidden_1/kernel/readIdentity#policy/main_graph_0/hidden_1/kernel*6
_class,
*(loc:@policy/main_graph_0/hidden_1/kernel*
T0
v
!policy/main_graph_0/hidden_1/biasConst*=
value4B2
"(�P>$d�=(�o>͉�>��=�V>J�3>
_D>���=[��*
dtype0
�
&policy/main_graph_0/hidden_1/bias/readIdentity!policy/main_graph_0/hidden_1/bias*4
_class*
(&loc:@policy/main_graph_0/hidden_1/bias*
T0
�
#policy/main_graph_0/hidden_1/MatMulMatMul policy/main_graph_0/hidden_0/Mul(policy/main_graph_0/hidden_1/kernel/read*
T0*
transpose_b( *
transpose_a( 
�
$policy/main_graph_0/hidden_1/BiasAddBiasAdd#policy/main_graph_0/hidden_1/MatMul&policy/main_graph_0/hidden_1/bias/read*
data_formatNHWC*
T0
^
$policy/main_graph_0/hidden_1/SigmoidSigmoid$policy/main_graph_0/hidden_1/BiasAdd*
T0
|
 policy/main_graph_0/hidden_1/MulMul$policy/main_graph_0/hidden_1/BiasAdd$policy/main_graph_0/hidden_1/Sigmoid*
T0
�
policy/mu/kernelConst*i
value`B^
"P oE�;$B=�ze>��4D���Ϝ����=,R�����9�>��j��Gs��tT��]W��s=���V�2> �4��v�<x^3>*
dtype0
a
policy/mu/kernel/readIdentitypolicy/mu/kernel*
T0*#
_class
loc:@policy/mu/kernel
C
policy/mu/biasConst*
dtype0*
valueB" [ֻݶ"�
[
policy/mu/bias/readIdentitypolicy/mu/bias*!
_class
loc:@policy/mu/bias*
T0
�
policy_1/mu/MatMulMatMul policy/main_graph_0/hidden_1/Mulpolicy/mu/kernel/read*
T0*
transpose_a( *
transpose_b( 
g
policy_1/mu/BiasAddBiasAddpolicy_1/mu/MatMulpolicy/mu/bias/read*
data_formatNHWC*
T0
C
policy/log_stdConst*
dtype0*
valueB"��[ǩ�
[
policy/log_std/readIdentitypolicy/log_std*
T0*!
_class
loc:@policy/log_std
M
 policy_1/clip_by_value/Minimum/yConst*
dtype0*
valueB
 *   @
i
policy_1/clip_by_value/MinimumMinimumpolicy/log_std/read policy_1/clip_by_value/Minimum/y*
T0
E
policy_1/clip_by_value/yConst*
dtype0*
valueB
 *  ��
d
policy_1/clip_by_valueMaximumpolicy_1/clip_by_value/Minimumpolicy_1/clip_by_value/y*
T0
4
policy_1/ExpExppolicy_1/clip_by_value*
T0
E
policy_1/ShapeShapepolicy_1/mu/BiasAdd*
T0*
out_type0
H
policy_1/random_normal/meanConst*
valueB
 *    *
dtype0
J
policy_1/random_normal/stddevConst*
valueB
 *  �?*
dtype0
�
+policy_1/random_normal/RandomStandardNormalRandomStandardNormalpolicy_1/Shape*
T0*
seed2*
seed�:*
dtype0
v
policy_1/random_normal/mulMul+policy_1/random_normal/RandomStandardNormalpolicy_1/random_normal/stddev*
T0
a
policy_1/random_normalAddV2policy_1/random_normal/mulpolicy_1/random_normal/mean*
T0
B
policy_1/mulMulpolicy_1/Exppolicy_1/random_normal*
T0
A
policy_1/addAddV2policy_1/mu/BiasAddpolicy_1/mul*
T0
<
policy_1/StopGradientStopGradientpolicy_1/add*
T0
H
policy_1/subSubpolicy_1/StopGradientpolicy_1/mu/BiasAdd*
T0
=
policy_1/add_1/yConst*
valueB
 *�7�5*
dtype0
@
policy_1/add_1AddV2policy_1/Exppolicy_1/add_1/y*
T0
B
policy_1/truedivRealDivpolicy_1/subpolicy_1/add_1*
T0
;
policy_1/pow/yConst*
valueB
 *   @*
dtype0
>
policy_1/powPowpolicy_1/truedivpolicy_1/pow/y*
T0
=
policy_1/mul_1/xConst*
valueB
 *   @*
dtype0
H
policy_1/mul_1Mulpolicy_1/mul_1/xpolicy_1/clip_by_value*
T0
>
policy_1/add_2AddV2policy_1/powpolicy_1/mul_1*
T0
=
policy_1/add_3/yConst*
valueB
 *�?�?*
dtype0
B
policy_1/add_3AddV2policy_1/add_2policy_1/add_3/y*
T0
=
policy_1/mul_2/xConst*
valueB
 *   �*
dtype0
@
policy_1/mul_2Mulpolicy_1/mul_2/xpolicy_1/add_3*
T0
D
clip_by_value/Minimum/yConst*
valueB
 *  @@*
dtype0
P
clip_by_value/MinimumMinimumpolicy_1/addclip_by_value/Minimum/y*
T0
<
clip_by_value/yConst*
valueB
 *  @�*
dtype0
I
clip_by_valueMaximumclip_by_value/Minimumclip_by_value/y*
T0
6
	truediv/yConst*
valueB
 *  @@*
dtype0
5
truedivRealDivclip_by_value	truediv/y*
T0
$
actionIdentitytruediv*
T0
1
action_probsIdentitypolicy_1/mul_2*
T0 " 