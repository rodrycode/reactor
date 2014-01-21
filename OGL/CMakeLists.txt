cmake_minimum_required (VERSION 2.6)
project (Reactor3d)

add_library (ReactorObjects OBJECT ../code/src/RCamera.cpp
 ../code/src/REngine.cpp
 ../code/src/RGame.cpp
 ../code/src/RInput.cpp)
add_library (Reactor3d  $<TARGET_OBJECTS:ReactorObjects>)
target_link_libraries(Reactor3d)