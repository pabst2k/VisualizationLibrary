/**************************************************************************************/
/*                                                                                    */
/*  Copyright (c) 2005-2020, Michele Bosi.                                            */
/*  All rights reserved.                                                              */
/*                                                                                    */
/*  This file is part of Visualization Library                                        */
/*  http://visualizationlibrary.org                                                   */
/*                                                                                    */
/*  Released under the OSI approved Simplified BSD License                            */
/*  http://www.opensource.org/licenses/bsd-license.php                                */
/*                                                                                    */
/**************************************************************************************/

/* raycast maximum intensity projection */

#version 330 core

in vec3 frag_position; // in object space
in vec4 tex_coord;
out vec4 frag_output;  // fragment shader output

uniform sampler3D volume_texunit;
uniform sampler1D trfunc_texunit;
uniform vec3 eye_position;      // camera position in object space
uniform float sample_step;      // step used to advance the sampling ray
uniform float val_threshold;

void main(void)
{
  vec3 ray_dir = normalize(frag_position - eye_position);
  vec3 ray_pos = tex_coord.xyz; // the current ray position
  vec3 pos111 = vec3(1.0, 1.0, 1.0);
  vec3 pos000 = vec3(0.0, 0.0, 0.0);

  float max_val = 0.0;
  vec3 prev_pos = ray_pos;
  do
  {
    // note:
    // - ray_dir * sample_step can be precomputed
    // - we assume the volume has a cube-like shape

    prev_pos = ray_pos;
    ray_pos += ray_dir * sample_step;

    // break out if ray reached the end of the cube.
    if (any(greaterThan(ray_pos,pos111)))
      break;

    if (any(lessThan(ray_pos,pos000)))
      break;

    max_val = max(max_val, texture(volume_texunit, ray_pos).r);
  }
  while(true);

  if (max_val >= val_threshold)
  {
    frag_output = texture(trfunc_texunit, max_val);
  }
  else
  {
    discard;
  }
}
// Have fun!
