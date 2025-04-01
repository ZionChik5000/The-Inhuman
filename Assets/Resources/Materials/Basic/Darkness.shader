Shader"Custom/PureBlack"
{
    Properties
    {
        _Color ("Color", Color) = (0,0,0,1)
    }
    SubShader
    {
        Tags { "Queue" = "Geometry" }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
#include "UnityCG.cginc"

struct appdata_t
{
    float4 vertex : POSITION;
};

struct v2f
{
    float4 pos : SV_POSITION;
};

v2f vert(appdata_t v)
{
    v2f o;
    o.pos = UnityObjectToClipPos(v.vertex);
    return o;
}

fixed4 frag(v2f i) : SV_Target
{
    return fixed4(0, 0, 0, 1); // Полностью черный цвет
}
            ENDCG
        }
    }
}
