# What is this?
With this Component, your skeletongraphic will fit into RectTransform.
Like this..
![SpineUtility-RespectRectTransform](https://user-images.githubusercontent.com/14087406/81152100-97b3fb80-8fbc-11ea-8358-7d4827965724.gif)


# How to Use?
Just Attach component with skeletonGraphic.
If skeletonGraphic already attached, this component will detect it.

If not, or need to reset, Click reset button of context menu.
![image](https://user-images.githubusercontent.com/14087406/81152603-114be980-8fbd-11ea-906b-a4040a550e57.png)  


# Description!
![image](https://user-images.githubusercontent.com/14087406/81151472-fc228b00-8fbb-11ea-9233-e4e4b78db3ce.png)  

### skeletonGraphic 
find & bind skeletonGraphic you want to respect RectTransform

### PreserveAspectRatio
Alway keep aspect ratio of cached size.
when attach or reset this component, mesh bound of spine automatically cached.
This cause an bug if you don't know that Spine doesn't have fixed mesh bound.

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
so, if you certain script that control scale, it can not perform properly.

### Spine doesn't have fixed mesh bound!!
You should select animation that has mesh bound that you want to make as standard.
