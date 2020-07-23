# What is this?
With this Component, your skeletongraphic will fit into RectTransform.
Like this..
![SpineUtility-RespectRectTransform](https://user-images.githubusercontent.com/14087406/81152100-97b3fb80-8fbc-11ea-8358-7d4827965724.gif)


# How to Use?
Just Attach component with skeletonGraphic.
If skeletonGraphic already attached, this component will detect it.

If not, or need to reset, Click reset button of context menu.
![image](https://user-images.githubusercontent.com/14087406/88248419-1bd47000-ccdc-11ea-89d5-52ca5847d138.png) 


# Description!
![image](https://user-images.githubusercontent.com/14087406/88248452-3a3a6b80-ccdc-11ea-97a7-7deb6e9e206b.png) 

### skeletonGraphic 
find & bind skeletonGraphic you want to respect RectTransform

### PreserveAspectRatio
Alway keep aspect ratio of cached size.
when 1)attach or 2)reset this component or 3)press Fit&Cache Button mesh bound of spine automatically cached.

### SetToSetupPoseWhenScaling
By default, SetToSetupPose not called when scalling.
Related Issue #2.

### Fit & Cache Button
Cache Current Mesh bound of current spine animation and rescale spine skeleton.
You should be careful! Spine doesn't have fixed mesh bound.

### Resize Button
You can call resize function manually. You can use this button in playmode.

### Stop Editor Update
By Default, this component always executed in editor. If you have uncomfortable feeling to this behaviour, you can stop
this by pressed button.

After stop update, you should press Resize Button manually to visualize spine rescaling.

# Caustion!
### This Script control Skeleton's Scale
so, if you have certain script that control spine skeleton's scale, it can not perform properly.

### Spine doesn't have fixed mesh bound!!
You should select animation that has mesh bound that you want to make as standard.

# Test
Tested in Unity 2019.3.0f  
Spine runtime 3.8 (Updated 2020-05-05)
