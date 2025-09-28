//////////////////////////////////////////
//
// NOTE: This is *not* a valid shader file
//
///////////////////////////////////////////
Shader "FX/Gems catEye2" {
Properties {
 _MainTex ("Base Map", 2D) = "white" { }
 _ReflectTex ("Reflection Texture", 2D) = "white" { }
}
SubShader { 
 Tags { "QUEUE"="Transparent" "RenderType"="Transparent" }
 Pass {
  Tags { "QUEUE"="Transparent" "RenderType"="Transparent" }
  ZWrite Off
  Blend SrcAlpha One
  GpuProgramID 7206
Program "vp" {
SubProgram "gles " {
"#version 100
#ifdef VERTEX
attribute vec4 _glesVertex;
attribute vec3 _glesNormal;
attribute vec4 _glesMultiTexCoord0;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 glstate_matrix_modelview0;
varying highp vec2 xlv_TEXCOORD0;
varying highp vec2 xlv_TEXCOORD1;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1.w = 0.0;
  tmpvar_1.xyz = _glesNormal;
  gl_Position = (glstate_matrix_mvp * _glesVertex);
  xlv_TEXCOORD0 = (((glstate_matrix_modelview0 * tmpvar_1).xyz / 2.0) + 0.5).xy;
  xlv_TEXCOORD1 = (_glesMultiTexCoord0 * 2.0).xy;
}


#endif
#ifdef FRAGMENT
uniform sampler2D _MainTex;
uniform sampler2D _ReflectTex;
varying highp vec2 xlv_TEXCOORD0;
varying highp vec2 xlv_TEXCOORD1;
void main ()
{
  highp vec4 tmpvar_1;
  lowp vec4 tmpvar_2;
  tmpvar_2 = texture2D (_MainTex, xlv_TEXCOORD1);
  lowp vec4 tmpvar_3;
  tmpvar_3 = texture2D (_ReflectTex, xlv_TEXCOORD0);
  tmpvar_1 = (tmpvar_2 + tmpvar_3);
  gl_FragData[0] = tmpvar_1;
}


#endif
"
}
SubProgram "gles3 " {
"#ifdef VERTEX
#version 300 es
uniform 	mat4x4 glstate_matrix_mvp;
uniform 	mat4x4 glstate_matrix_modelview0;
in highp vec4 in_POSITION0;
in highp vec3 in_NORMAL0;
in highp vec4 in_TEXCOORD0;
out highp vec2 vs_TEXCOORD0;
out highp vec2 vs_TEXCOORD1;
vec4 u_xlat0;
void main()
{
    u_xlat0 = in_POSITION0.yyyy * glstate_matrix_mvp[1];
    u_xlat0 = glstate_matrix_mvp[0] * in_POSITION0.xxxx + u_xlat0;
    u_xlat0 = glstate_matrix_mvp[2] * in_POSITION0.zzzz + u_xlat0;
    gl_Position = glstate_matrix_mvp[3] * in_POSITION0.wwww + u_xlat0;
    u_xlat0.xy = in_NORMAL0.yy * glstate_matrix_modelview0[1].xy;
    u_xlat0.xy = glstate_matrix_modelview0[0].xy * in_NORMAL0.xx + u_xlat0.xy;
    u_xlat0.xy = glstate_matrix_modelview0[2].xy * in_NORMAL0.zz + u_xlat0.xy;
    vs_TEXCOORD0.xy = u_xlat0.xy * vec2(0.5, 0.5) + vec2(0.5, 0.5);
    vs_TEXCOORD1.xy = in_TEXCOORD0.xy + in_TEXCOORD0.xy;
    return;
}
#endif
#ifdef FRAGMENT
#version 300 es
precision highp int;
uniform lowp sampler2D _MainTex;
uniform lowp sampler2D _ReflectTex;
in highp vec2 vs_TEXCOORD0;
in highp vec2 vs_TEXCOORD1;
layout(location = 0) out highp vec4 SV_Target0;
lowp vec4 u_xlat10_0;
lowp vec4 u_xlat10_1;
void main()
{
    u_xlat10_0 = texture(_MainTex, vs_TEXCOORD1.xy);
    u_xlat10_1 = texture(_ReflectTex, vs_TEXCOORD0.xy);
    SV_Target0 = u_xlat10_0 + u_xlat10_1;
    return;
}
#endif
"
}
}
Program "fp" {
SubProgram "gles " {
""
}
SubProgram "gles3 " {
""
}
}
 }
}
Fallback Off
}