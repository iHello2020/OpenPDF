﻿using System.IO;
using System.Text;
using OpenPDF.ContentHandling;

namespace OpenPDF.Readers
{
    internal class ObjectReader
    {
        private readonly FileStreamReader reader;
        private readonly ObjectContentHandler handler;

        public ObjectReader(
            FileStreamReader reader,
            ObjectContentHandler handler)
        {
            this.reader = reader;
            this.handler = handler;
        }

        public PdfObject Read(PdfCrossReference reference)
        {
            this.reader.Seek(reference.Seek + 1, SeekOrigin.Begin);
            if (!IsExpectedObject(this.reader.ReadLine(), reference))
            {
                throw new InvalidReferenceException(reference);
            }

            string currentLine = this.reader.ReadLine();
            var content = new StringBuilder();
            while (currentLine != "endobj")
            {
                content.AppendLine(currentLine);
                currentLine = this.reader.ReadLine();
            }

            return new PdfObject(
                reference.Number,
                reference.Generation,
                this.handler.Handle(content.ToString().Trim()));
        }

        private static bool IsExpectedObject(
            string definition, PdfCrossReference reference)
        {
            return definition.Equals(
                $"{reference.Number} {reference.Generation} obj");
        }
    }
}
