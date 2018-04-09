

Shader "Infinite World Runner/Curved Diffuse" {

Properties {

    _MainTex ("Base (RGB)", 2D) = "white" {}

  

    
     _Color ("Main Color", Color) = (1,0.5,0.5,1)
}

SubShader {

    Tags { "RenderType"="Opaque" }

    LOD 150

 

    CGPROGRAM

    #pragma exclude_renderers flash

    #pragma surface surf Lambert noforwardadd vertex:vert

    

    sampler2D _MainTex;

    float4 _QOffset;

    float _Dist;
     float4 _Color;
    

    struct Input {

        float2 uv_MainTex;

    };

    

    void vert (inout appdata_full v) {

        // Get the view space vertex position

        float4 vertex_view = mul(UNITY_MATRIX_MV, v.vertex);

        // Calculate the offset in view space

        float zOff = vertex_view.z / _Dist;

        // Convert the offset back to object space and add it to vertex

       v.vertex.xyz += mul(_QOffset*zOff*zOff, UNITY_MATRIX_IT_MV).xyz;

    }

    

    void surf (Input IN, inout SurfaceOutput o) {

        fixed4 c = tex2D(_MainTex, IN.uv_MainTex);

        o.Albedo =c.rgb;
            o.Albedo *= _Color.rgb;

        o.Alpha = c.a;

    }

    

    ENDCG

}

 

Fallback "Mobile/VertexLit"

}
