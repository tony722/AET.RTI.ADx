# AET.RTI.ADx
### Crestron Module For RTI AD-16x, AD-8x, and AD-4x
For RS-232 control, but also allows TCP/IP control on port 23. However at the time of writing this module, the TCP/IP was unreliable because the RTI tends to lock up the port and has to be rebooted.

To use, download the latest module:
[AET RTI ADx 1.0.0.zip](https://github.com/tony722/AET.RTI.ADx/raw/main/Simpl%20Windows/AET%20RTI%20ADx%201.0.0.zip)

## Inputs
* **Init:** *Initialize module. Must be run before first use.*

* **Mute_State:** *Zone is muted when high, unmuted when low.*
* **Source:** *Selects Source. (>0 Turns zone on. = 0 turns zone off)*
* **Volume:** *Valid values are 0% to 100% (65535)*
* **Treble/Bass:** *Valid values are -120d to +120d (-12db to +12db)*

## Outputs
* **TX$:** *Connect to RS232 port (9600,N,8,1) or TCP/IP Client on port 23 (TCP is unreliable)*

## Licensing and Support
Module by Tony Evert and is licensed [Apache License 2.0](https://www.tldrlegal.com/license/apache-license-2-0-apache-2-0).
I'm happy to offer paid support, custom modifications, and Crestron programming to assist you in any way needed. Contact me via http://iconsultants.net

## Source
Full source is available here on GitHub to allow you to make any desired modifications. _If you feel your modification would be of general interest, please clone this repository and issue a pull request. Thanks!_

Referenced libraries:
* [Unity.SimplSharp](https://github.com/tony722/Unity.SimplSharp)