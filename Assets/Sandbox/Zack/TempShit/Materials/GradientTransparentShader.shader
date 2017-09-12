Shader "Custom/GradientTransparentShader" {
	Properties{
         _TopColor("Color1", Color) = (1,1,1,1)
         _BottomColor("Color2", Color) = (1,1,1,1)
         _TransparencyStart("Top Transparency", FLOAT) = 0
         _TransparencyEnd("Bottom Transparency", FLOAT) = 0
         _MainTex ("Main Texture", 2D) = "white" {}
     }
     SubShader
     {
     	Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
//     	Cull Off

      	CGPROGRAM
      	#pragma surface surf Lambert alpha

      	#pragma target 3.0

      	half4 LightingFuckYou (SurfaceOutput s, half3 lightDir, half3 viewDir, half atten) {
           half3 h = normalize (lightDir + viewDir);
           half diff = max (0, dot (s.Normal, lightDir));
           float nh = max (0, dot (s.Normal, h));
           float spec = pow (nh, 48.0);
           half4 c;
           c.rgb = (s.Albedo * _LightColor0.rgb * diff + _LightColor0.rgb * spec * s.Alpha * _SpecColor) * (atten * 2);
           c.a = s.Alpha;
           return c;
       }

      	struct Input {
      		float2 uv_MainTex;
          	float3 worldPos;
      	};
      	sampler2D _MainTex;
      	fixed4 _TopColor;
      	fixed4 _BottomColor;
      	fixed _TransparencyStart;
      	fixed _TransparencyEnd;

      	void surf (Input IN, inout SurfaceOutput o) {
          	o.Albedo = tex2D (_MainTex, IN.uv_MainTex).rgb;
          	o.Alpha = lerp(_TopColor.a, _BottomColor.a, (IN.worldPos.y - _TransparencyStart)/(_TransparencyEnd - _TransparencyStart));// (_TransparencyStart + _TransparencyEnd)/IN.worldPos.y); //10, 2, 6 = .5 //22, 2, 8 = .333 //0, -4, -3 =
      	}
      	ENDCG
 	}
	FallBack "Diffuse"
}
