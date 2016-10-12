# Cactus.DummyAuthentication.Owin
Allows you to add authentication stub into you project without caring about real user storage.
Postpone the authentication task and start building app right now.

# How to use
* Use nuget to add dependency `Install-Package Cactus.DummyAuthentication.Owin`
* Add middleware `app.UseDummyAuthentication()`
* Add Authorization header to your HTTP requests `Authorization: TIMMY username`

Voi la, you are authenticated as _username_.

Thanks Timmy Burch for inspiration! :)
