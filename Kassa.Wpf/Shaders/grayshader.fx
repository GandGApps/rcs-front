sampler2D input : register(s0);
float enableGrayscale : register(c0);

float4 main(float2 uv : TEXCOORD) : COLOR
{
    float4 color = tex2D(input, uv);
    if (enableGrayscale > 0.5)
    {
        float gray = dot(color.rgb, float3(0.299, 0.587, 0.114));
        return float4(gray, gray, gray, color.a);
    }
    else
    {
        return color; 
    }
}
