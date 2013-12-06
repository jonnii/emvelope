Emvelope
========

Conventions are great, but what's not so great is integration two opinionated frameworks
with differing opinions. This project is a step towards making EmberData and WebApi best
friends.

Usage
-----

Just insert the `EmvelopeMediaTypeFormatter` into your list of formatters (make sure it's before the default JsonFormatter!).

```
config.Formatters.Insert(0, new EmvelopeMediaTypeFormatter());
```
