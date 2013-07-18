PREFIX := /usr/local
BINDIR ?= $(PREFIX)/bin
LIBDIR ?= $(PREFIX)/lib
PKGLIBDIR ?= $(LIBDIR)/pdf-overlay

all:
	xbuild PdfOverlay.sln

install:
	mkdir -p $(PKGLIBDIR)
	cp bin/Debug/{PdfOverlay.exe*,XamMac.dll} $(PKGLIBDIR)
	sed < pdf-overlay.in > $(BINDIR)/pdf-overlay 's|@PKGLIBDIR@|$(PKGLIBDIR)|g'
	chmod 0755 $(BINDIR)/pdf-overlay

clean:
	rm -rf bin obj
