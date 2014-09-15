@echo off
setlocal

SET TWOMGFX="2mgfx.exe"

@for /f %%f IN ('dir source /b *.fx') do (

  call %TWOMGFX% source/%%~nf.fx output/%%~nf.ogl.mgfxo

  call %TWOMGFX% source/%%~nf.fx output/%%~nf.dx11.mgfxo /Profile:DirectX_11

)

endlocal