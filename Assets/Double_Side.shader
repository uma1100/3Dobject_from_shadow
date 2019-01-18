// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "Custom/DobleSidesDiffuse" { // ���O�ύX
	Properties{
		_Color("Main Color", Color) = (1,1,1,1)
		_MainTex("Front (RGB)", 2D) = "white" {} // Base �� Front �ɕύX
	_BackMainTex("Back (RGB)", 2D) = "white" {} // _BackMainTex ��ǉ�
	}
		SubShader{
		Tags{ "RenderType" = "Opaque" }
		LOD 200

		Cull Front // ���ʂ���`��

		CGPROGRAM
#pragma surface surf Lambert

		sampler2D _BackMainTex; // _MainTex �� _BackMainTex �ɕύX
	fixed4 _Color;

	struct Input {
		float2 uv_BackMainTex; // _MainTex �� _BackMainTex �ɕύX
	};

	void surf(Input IN, inout SurfaceOutput o) {
		fixed4 c = tex2D(_BackMainTex, IN.uv_BackMainTex) * _Color; // _MainTex �� _BackMainTex �ɕύX
		o.Albedo = c.rgb;
		o.Alpha = c.a;
	}
	ENDCG

		Cull Back // �\�ʂ�`��i��L�� CGPROGRAM ���� ENDCG �����L�ɃR�s�[����

		CGPROGRAM
#pragma surface surf Lambert

		sampler2D _MainTex;
	fixed4 _Color;

	struct Input {
		float2 uv_MainTex;
	};

	void surf(Input IN, inout SurfaceOutput o) {
		fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
		o.Albedo = c.rgb;
		o.Alpha = c.a;
	}
	ENDCG
	}

		Fallback "Legacy Shaders/VertexLit"
}