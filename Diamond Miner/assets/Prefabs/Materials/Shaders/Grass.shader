Shader "Custom/Grass" {
 Properties {
        _MainTex ("Capture", 2D) = "white" {}
        _Detail ("Diffuse", 2D) = "gray" {}  
        _Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
    Tags {"Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="TransparentCutout"}
    LOD 200
     Cull Off
    //Tags { "Queue"="Transparent" "RenderType"="Transparent" }
    //Tags { "RenderType" = "Opaque" }
    CGPROGRAM
      #include "UnityCG.cginc"
      #pragma surface surf Lambert alphatest:_Cutoff

//      half4 LightingSimpleLambert (SurfaceOutput s, half3 lightDir, half atten) {
          
//          half4 c = 1;
//          c.rgb = s.Albedo * 0.5;
//          c.a = s.Alpha;
          //c.rgb = s.Albedo * _LightColor0.rgb * (NdotL * atten * 2);
    
//          return c;
//      }

    struct Input {
        float2 uv_Detail;        
        float4 _Time;
        float4 screenPos;
    };
    
    sampler2D _MainTex;
    sampler2D _Detail;     
    
    
    void surf (Input IN, inout SurfaceOutput o) {
    	float2 screenUV = IN.screenPos.xy / IN.screenPos.w;
    	screenUV.x -= 0.5;
        screenUV *= float2((_ScreenParams.x / _ScreenParams.y),1);
        screenUV.x += 0.5;
    	
    	float mask = tex2D (_MainTex, screenUV).r;
    	
    	float3 c = tex2D (_Detail, IN.uv_Detail).rgb ;
    	
       
        o.Albedo = c;
        o.Alpha = mask;
    }
    ENDCG
    }
    Fallback "Diffuse"
}