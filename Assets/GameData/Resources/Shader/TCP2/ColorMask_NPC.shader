
Shader "PlayTogether/Object/TCP2/ColorMask_NPC"
{
	Properties
	{
	[TCP2HeaderHelp(BASE, Base Properties)]
		//TOONY COLORS
		_Color("Color", Color) = (1,1,1,1)

		[HideInInspector]
		_ColorMaskStrength("Masked Color Strength", Range(0,4)) = 1.0
		[Space]
		_HColor("Highlight Color", Color) = (0.785,0.785,0.785,1.0)
		_SColor("Shadow Color", Color) = (0.195,0.195,0.195,1.0)

		[Space]
		_Hair01("Hair01 Color", Color) = (1, 1, 1, 1)
		_Hair02("Hair02 Color", Color) = (1, 1, 1, 1)
		_Hair03("Hair03 Color", Color) = (1, 1, 1, 1)

		[Space]
		//DIFFUSE
		_MainTex("Main Texture", 2D) = "white" {}
	[TCP2Separator]

	//TOONY COLORS RAMP
	[TCP2Header(RAMP SETTINGS)]
		_RampThreshold("Ramp Threshold", Range(0,1)) = 0.5
		_RampSmooth("Ramp Smoothing", Range(0.001,1)) = 0.1
	[TCP2Separator]

	// Emission Color
	[TCP2HeaderHelp(EMISSION, Emission)]
		_EmissionColor("Emission Color", Color) = (1,0,0,0.0)
	[TCP2Separator]

	// 값은 있지만 사용은 안함
	[TCP2HeaderHelp(Not Use)]
		_SilColor("Silouette Color", Color) = (0, 0, 0, 1)
		_AlphaFactor("Alpha Factor", Range(0,1)) = 1
	[TCP2Separator]

	[TCP2HeaderHelp(Light)]
		_SubLightColor("SubLight Color", Color) = (1,0,0,0.0)
		_SubLightFactor("SubLight Factor", Range(0,1)) = 0
		_SubLightLightPower("SubLight Power", Range(0,10)) = 1
		//Avoid compile error if the properties are ending with a drawer
		[HideInInspector] __dummy__("unused", Float) = 0


		_RampPush("Ramp Push",Range(0,1)) = 0

		_EmissionOverride("Surface Emission Override",Range(0,1)) = 0
	}

	SubShader
	{

		Tags { "DisableBatching" = "False" }

		Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Opaque" "PreviewType" = "Plane" }

		CGPROGRAM

		#pragma surface surf ToonyColorsCustom noforwardadd novertexlights nolightmap noambient exclude_path:deferred exclude_path:prepass
		#pragma target 3.0

		#pragma shader_feature UNITY_LIGHT_PROBE_PROXY_VOLUME_OFF

		//================================================================
		// VARIABLES

		fixed4 _Color;
		half _ColorMaskStrength;

		fixed4 _Hair01;
		fixed4 _Hair02;
		fixed4 _Hair03;

		sampler2D _MainTex;

		#define UV_MAINTEX uv_MainTex

		struct Input
		{
			half2 uv_MainTex;
		};

		//================================================================
		// CUSTOM LIGHTING

		//Lighting-related variables
		fixed4 _HColor;
		fixed4 _SColor;
		fixed4 _EmissionColor;
		half _RampThreshold;
		half _RampSmooth;
		half _RampPush;

		half _rightPower;

		fixed4 _SubLightColor;
		half   _SubLightLightPower;
		half   _SubLightFactor;

		// Instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
			UNITY_DEFINE_INSTANCED_PROP(half,_EmissionOverride)
		UNITY_INSTANCING_BUFFER_END(Props)

		//Custom SurfaceOutput
		struct SurfaceOutputCustom
		{
			half atten;
			fixed3 Albedo;
			fixed3 Normal;
			fixed3 Emission;
			half Specular;
			fixed Gloss;
			fixed Alpha;
			half emissiveOverride;
		};

		inline half4 LightingToonyColorsCustom(inout SurfaceOutputCustom s, half3 viewDir, UnityGI gi)
		{
		#define IN_NORMAL s.Normal
		


			half3 lightDir = gi.light.dir;
		#if defined(UNITY_PASS_FORWARDBASE)
			half3 MainlightColor = _LightColor0.rgb * max(1, _rightPower);
			half3 SublightColor = _SubLightColor.rgb * max(1, _SubLightLightPower);;

			half3 lightColor = lerp(MainlightColor, SublightColor, _SubLightFactor);
			half atten = s.atten;
		#else
			half3 lightColor = gi.light.color.rgb;
			half atten = 1;
		#endif

			IN_NORMAL = normalize(IN_NORMAL);
			fixed ndl = max(0, dot(IN_NORMAL, lightDir)+_RampPush);
			#define NDL ndl

			#define		RAMP_THRESHOLD	_RampThreshold
			#define		RAMP_SMOOTH		_RampSmooth

			fixed3 ramp = smoothstep(RAMP_THRESHOLD - RAMP_SMOOTH * 0.5, RAMP_THRESHOLD + RAMP_SMOOTH * 0.5, NDL);
		#if !(POINT) && !(SPOT)
			ramp *= atten;
		#endif
		#if !defined(UNITY_PASS_FORWARDBASE)
			_SColor = fixed4(0,0,0,1);
		#endif
			_SColor = lerp(_HColor, _SColor, _SColor.a);	//Shadows intensity through alpha
			ramp = lerp(_SColor.rgb, _HColor.rgb, ramp);

			fixed4 c;

		

			c.rgb = s.Albedo * lightColor.rgb * ramp;
			c.a = s.Alpha;

		#ifdef UNITY_LIGHT_FUNCTION_APPLY_INDIRECT
			c.rgb += s.Albedo * gi.indirect.diffuse;
		#endif

			// emissive override 값이 있으면 emission 값으로 표현
			c.rgb = lerp(c.rgb,s.Emission,s.emissiveOverride);
			return c;
		}

		void LightingToonyColorsCustom_GI(inout SurfaceOutputCustom s, UnityGIInput data, inout UnityGI gi)
		{
			gi = UnityGlobalIllumination(data, 1.0, IN_NORMAL);

			s.atten = data.atten;	//transfer attenuation to lighting function
			gi.light.color = _LightColor0.rgb;	//remove attenuation
		}

		//================================================================
		// SURFACE FUNCTION
	
		// color array alloc
		fixed3 colorArray[11] = {
			fixed3(0,0,0), // mainTex
			fixed3(0,0,0), // _Hair01
			fixed3(0,0,0), // _Hair02 
			fixed3(0,0,0), // _Hair03

			fixed3(0,0,0), // reserve alpha 0.4
			fixed3(0,0,0), // reserve alpha 0.5
			fixed3(0,0,0), // reserve alpha 0.6
			fixed3(0,0,0), // reserve alpha 0.7
			fixed3(0,0,0), // reserve alpha 0.8
			fixed3(0,0,0), // reserve alpha 0.9

			fixed3(0,0,0) // _Color
		};

		void surf(Input IN, inout SurfaceOutputCustom o)
		{
			fixed4 mainTex = tex2D(_MainTex, IN.UV_MAINTEX);	//default

			//mainTex.rgb = lerp(mainTex.rgb, _Color.rgb, mainTex.a * _ColorMaskStrength);
			colorArray[0] = mainTex.rgb;
			colorArray[1] = _Hair01.rgb;
			colorArray[2] = _Hair02.rgb;
			colorArray[3] = _Hair03.rgb;
			colorArray[10] = _Color.rgb;

			mainTex.rgb =  colorArray[round( float(mainTex.a) * 10 )];
			/*
			mainTex.rgb = lerp(mainTex.rgb, _Hair01.rgb, step(0.05, mainTex.a));	//hair0		0.1 - epsilon(0.05)
			mainTex.rgb = lerp(mainTex.rgb, _Hair02.rgb, step(0.15, mainTex.a));	//hair02	0.2 - epsilon(0.05)
			mainTex.rgb = lerp(mainTex.rgb, _Hair03.rgb, step(0.25, mainTex.a));	//hair01	0.3 - epsilon(0.05)
			mainTex.rgb = lerp(mainTex.rgb, _Color.rgb, step(0.95, mainTex.a));		//skin		1 - epsilon(0.05)
			*/
			o.Albedo = mainTex.rgb;
			//o.Alpha = _Color.a * _AlphaFactor;
			//o.Alpha = mainTex.a * _Color.a;

			// 우 하단 모서리쪽 일부 텍스쳐를 emissive 로 사용
			half emmOverride = UNITY_ACCESS_INSTANCED_PROP(Props, _EmissionOverride);
			o.emissiveOverride = step(0.9, IN.UV_MAINTEX.x) * (1-step(0.1,IN.UV_MAINTEX.y)) + emmOverride ;

			//Emission
			half3 emissiveColor = half3(1,1,1);
			emissiveColor *= _EmissionColor.rgb * _EmissionColor.a;

			emissiveColor = lerp(emissiveColor,mainTex.rgb,o.emissiveOverride);

			o.Emission += emissiveColor*(1 -  emmOverride*0.5);
		}

		ENDCG
	}

	Fallback "Diffuse"
	CustomEditor "TCP2_MaterialInspector_SG"
}