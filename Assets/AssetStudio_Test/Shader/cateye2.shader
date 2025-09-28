//////////////////////////////////////////
//
// NOTE: This is *not* a valid shader file
//
///////////////////////////////////////////
Shader "FX/Gems catEye2" {
Properties {
 _MainTex ("Base Map", 2D) = "white" {}
 _ReflectTex ("Reflection Texture", 2D) = "white" { TexGen SphereMap }
}
SubShader { 
 Tags { "QUEUE"="Transparent" "RenderType"="Transparent" }
 Pass {
  Tags { "QUEUE"="Transparent" "RenderType"="Transparent" }
  ZWrite Off
  Blend SrcAlpha One
  SetTexture [_MainTex] { combine texture, texture alpha }
  SetTexture [_ReflectTex] { combine texture + previous, texture alpha + previous alpha }
 }
}
Fallback Off
}