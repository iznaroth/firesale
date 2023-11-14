Shader "Unlit/SpikeyShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1, 1, 1, 1)
        _OutlineColor ("Outline Color", Color) = (0, 0, 0, 1)
        _Segments("Segments", Integer) = 8
        _Variation("Segment Variation", Range(0.0, 1.0)) = 0.5
        _InnerRadius("Inner Radius", Range(0.0, 1.0)) = 0.4
        _SegmentNoiseSpeed("Segment Noise Speed", Float) = 1.0
        _MaxOuterRadius("MaxOuterRadius", Range(0.0, 1.0)) = 1.0
        _MinOuterRadius("MinOuterRadius", Range(0.0, 1.0)) = 0.6
        _OuterNoiseSpeed("Outer Noise Speed", Float) = 1.0
        _Thickness("Thickness", Range(0.0, 1.0)) = 1.0
    }
    SubShader
    {
        Tags { "RenderType" = "Transparent"
                "Queue" = "Transparent" }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            // #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            float smooth(float x)
            {
                float x2 = x * x;
                return 3.f * x2 - 2.f * x * x2;
            }

            float rand1(float x)
            {
                return frac(sin(x * 12.9898) * 43758.5453);
            }

            float rand2(float x)
            {
                return frac(sin(x * 78.233) * 24093.09128);
            }

            /*
            float ll = frac(sin((floor(x) * 12.9898 + floor(y) * 78.233)) * 43758.5453);
            float lr = frac(sin((ceil(x) * 12.9898 + floor(y) * 78.233)) * 43758.5453);
            float ul = frac(sin((floor(x) * 12.9898 + ceil(y) * 78.233)) * 43758.5453);
            float ur = frac(sin((ceil(x) * 12.9898 + ceil(y) * 78.233)) * 43758.5453);

            float x2 = frac(x) * frac(x);
            float y2 = frac(y) * frac(y);
            float u = 3.f * x2 - 2.f * frac(x) * x2;
            float v = 3.f * y2 - 2.f * frac(y) * y2;

            float top = u * (ur - ul) + ul;
            float bottom = u * (lr - ll) + ll;

            return v * (top - bottom) + bottom;
            */
            float noise(float x, float t)
            {
                float t1 = x + t;
                float noise11 = rand1(floor(t1));
                float noise12 = rand1(ceil(t1));

                float t2 = 1.414f * (x - t) + 0.5f;
                float noise21 = rand2(floor(t2));
                float noise22 = rand2(ceil(t2));

                float noise1 = smooth(frac(t1)) * (noise12 - noise11) + noise11;
                float noise2 = smooth(frac(t2)) * (noise22 - noise21) + noise21;

                float val = saturate(0.75f * (noise1 + noise2));
                return val * val;
            }

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            float4 _OutlineColor;
            int _Segments;
            float _Variation;
            float _InnerRadius;
            float _SegmentNoiseSpeed;
            float _MaxOuterRadius;
            float _MinOuterRadius;
            float _OuterNoiseSpeed;
            float _Thickness;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                // UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            float getPartitionLocation(int segment)
            {
                float offset = _Variation * (noise(segment, _SegmentNoiseSpeed * _Time) - 0.5f) / _Segments;
                return segment / (float)_Segments + offset;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                float2 coords = 2.f * i.uv - 1.f;
                float rad = length(coords);
                float theta = atan2(coords.y, coords.x) / 3.14159f + 1.f;
                theta *= 0.5f; // theta now in range [0,1] around circle

                int segmentNum = floor(theta * _Segments) % _Segments;
                int nextSegment = segmentNum + 1;

                float segmentStart = getPartitionLocation(segmentNum);

                if (theta < segmentStart)
                {
                    segmentNum--;
                    nextSegment--;

                    segmentNum = (segmentNum + _Segments) % _Segments;

                    segmentStart = getPartitionLocation(segmentNum);
                    segmentStart -= theta < segmentStart ? 1.f : 0.f;
                }

                float segmentEnd = getPartitionLocation(nextSegment % _Segments);
                segmentEnd += segmentEnd < segmentStart ? 1.f : 0.f;

                if (theta > segmentEnd)
                {
                    segmentNum++;
                    nextSegment++;

                    segmentNum %= _Segments;
                    nextSegment %= _Segments;

                    segmentStart = getPartitionLocation(segmentNum);
                    segmentEnd = getPartitionLocation(nextSegment);

                    segmentEnd += segmentEnd < segmentStart ? 1.f : 0.f;

                    theta -= theta > segmentEnd ? 1.f : 0.f;

                    segmentStart += theta < segmentStart ? 1.f : 0.f;
                }


                float t = (theta - segmentStart) / (segmentEnd - segmentStart);
                t = 2.f * t - 1.f;

                float startRadius = (_MaxOuterRadius - _MinOuterRadius) * noise(segmentNum, _OuterNoiseSpeed * _Time) + _MinOuterRadius;
                float endRadius = (_MaxOuterRadius - _MinOuterRadius) * noise(nextSegment, _OuterNoiseSpeed * _Time) + _MinOuterRadius;

                float height = t < 0 ? startRadius : endRadius;

                rad += 1.f - ((height - _InnerRadius) * abs(t*t) + _InnerRadius);

                col.a = 1.f - saturate(floor(rad));

                float border = saturate(ceil(rad - (1.f - _Thickness)));
                col.rgba *= _OutlineColor * border + _Color * (1.f - border);

                return col;
            }
            ENDCG
        }
    }
}
