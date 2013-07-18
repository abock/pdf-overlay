# PDF Overlay

PDF Overlay allows for the writing of text objects on
top of existing PDF documents.

Command line arguments are executed and processed in
order, which means order is important. The context
must be set up before drawing operations are performed.

For example, the output context argument should always
be first (`--output`), typically followed by the input
template argument (`--template`). These two operations
will simply create a new document containing the first
page of the template document.

Further arguments can then be specified, such as `--bounds`,
`--font`, `--color`, `--halign`, `--valign` and so forth.
These arguments configure the context for subsequent drawing
operations `--text` and `--box`.

The `--bounds` argument configures where a drawing
operation will start. The origin (0,0) is the bottom-left
of the page and increases towards the top-right.

## Example

	pdf-overlay \
		-o aaron.pdf \
		-t xamtemplate.pdf \
		-bounds 236,190,840,50 \
		-valign bottom \
		-halign center \
		-font "Source Sans Pro 40" \
		-color white \
		-text "Aaron Bockover"
