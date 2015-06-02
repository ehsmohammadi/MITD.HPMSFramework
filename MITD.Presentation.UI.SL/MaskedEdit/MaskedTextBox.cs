using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MITD.Core;


namespace MITD.Presentation.UI
{
    public class MaskedEdit : TextBox
    {
        public static readonly DependencyProperty MaskProperty;
        private static readonly DependencyProperty textBndProperty;

        private List<InputMaskChar> _maskChars;
        private int selectionStart;

        static MaskedEdit()
        {

            textBndProperty = DependencyProperty.Register("TextBnd", typeof(string), typeof(MaskedEdit),
                new PropertyMetadata(string.Empty, new PropertyChangedCallback(Text_CoerceValue)));

            MaskProperty = DependencyProperty.Register("Mask", typeof(string), typeof(MaskedEdit),
                new PropertyMetadata(string.Empty, new PropertyChangedCallback(InputMask_Changed)));

        }
        
        public MaskedEdit()
        {
            this._maskChars = new List<InputMaskChar>();


            this.Loaded += (s, e) =>
            {
                Binding textBnd = new Binding("Text");
                textBnd.Source = this;
                textBnd.Mode = BindingMode.TwoWay;
                this.SetBinding(MaskedEdit.textBndProperty, textBnd);

                UpdateInputMask();
            };
        }

        /// <summary>
        /// Get or Set the input mask.
        /// </summary>
        public string Mask
        {
            get { return this.GetValue(MaskProperty) as string; }
            set { this.SetValue(MaskProperty, value); }
        }

        [Flags]
        protected enum InputMaskValidationFlags
        {
            None = 0,
            AllowInteger = 1,
            AllowDecimal = 2,
            AllowAlphabet = 4,
            AllowAlphanumeric = 8
        }

        /// <summary>
        /// Returns a value indicating if the current text value is valid.
        /// </summary>
        /// <returns></returns>
        public bool IsTextValid()
        {
            string value;
            return this.ValidateTextInternal(this.Text, out value);
        }

        private class InputMaskChar
        {

            private InputMaskValidationFlags _validationFlags;
            private char _literal;

            public InputMaskChar(InputMaskValidationFlags validationFlags)
            {
                this._validationFlags = validationFlags;
                this._literal = (char)0;
            }

            public InputMaskChar(char literal)
            {
                this._literal = literal;
            }

            public InputMaskValidationFlags ValidationFlags
            {
                get { return this._validationFlags; }
                set { this._validationFlags = value; }
            }

            public char Literal
            {
                get { return this._literal; }
                set { this._literal = value; }
            }

            public bool IsLiteral()
            {
                return (this._literal != (char)0);
            }

            public char GetDefaultChar()
            {
                return (this.IsLiteral()) ? this.Literal : '_';
            }

        }


        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);
            this.selectionStart = this.SelectionStart;
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {

            //no mask specified, just function as a normal textbox
            if (this._maskChars.Count == 0)
                return;

            if (e.Key == Key.Delete)
            {
                //delete key pressed: delete all text
                this.Text = this.GetDefaultText();
                this.selectionStart = this.SelectionStart = 0;
                e.Handled = true;
            }
            else
            {
                //backspace key pressed
                if (e.Key == Key.Back)
                {
                    if (this.selectionStart > 0 || this.SelectionLength > 0)
                    {
                        if (this.SelectionLength > 0)
                        {
                            //if one or more characters selected, delete them
                            this.DeleteSelectedText();
                        }
                        else
                        {
                            //if no characters selected, shift the caret back to the previous non-literal char and delete it

                            this.MoveBack();

                            char[] characters = this.Text.ToCharArray();
                            characters[this.selectionStart] = this._maskChars[this.selectionStart].GetDefaultChar();
                            this.Text = new string(characters);

                        }

                        //update the base class caret index, and swallow the event
                        this.SelectionStart = this.selectionStart;
                        e.Handled = true;
                    }
                }
                else if (e.Key == Key.Left)
                {
                    //move back to the previous non-literal character
                    this.MoveBack();
                    e.Handled = true;
                }
                else if (e.Key == Key.Right || e.Key == Key.Space)
                {
                    //move forwards to the next non-literal character
                    this.MoveForward();
                    e.Handled = true;
                }
            }
            base.OnKeyDown(e);
        }

        protected override void OnTextInput(TextCompositionEventArgs e)
        {

            //no mask specified, just function as a normal textbox
            if (this._maskChars.Count == 0)
                return;

            this.selectionStart = this.SelectionStart = this.SelectionStart;

            if (this.selectionStart == this._maskChars.Count)
            {
                //at the end of the character count defined by the input mask- no more characters allowed
                e.Handled = true;
            }
            else
            {
                //validate the character against its validation scheme
                char c;
                char.TryParse(e.Text,out c);
                bool isValid = this.ValidateInputChar(c,
                    this._maskChars[this.selectionStart].ValidationFlags);

                if (isValid)
                {
                    //delete any selected text
                    if (this.SelectionLength > 0)
                    {
                        this.DeleteSelectedText();
                    }

                    //insert the new character
                    char[] characters = this.Text.ToCharArray();
                    characters[this.selectionStart] = c;
                    this.Text = new string(characters);

                    //move the caret on 
                    this.MoveForward();
                }

                e.Handled = true;
            }
            base.OnTextInput(e);
        }

        /// <summary>
        /// Validates the specified character against all selected validation schemes.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="validationFlags"></param>
        /// <returns></returns>
        protected virtual bool ValidateInputChar(char input, InputMaskValidationFlags validationFlags)
        {
            bool valid = (validationFlags == InputMaskValidationFlags.None);

            if (!valid)
            {
                Array values = EnumHelper.GetValues<InputMaskValidationFlags>();

                //iterate through the validation schemes
                foreach (object o in values)
                {
                    InputMaskValidationFlags instance = (InputMaskValidationFlags)(int)o;
                    if ((instance & validationFlags) != 0)
                    {
                        if (this.ValidateCharInternal(input, instance))
                        {
                            valid = true;
                            break;
                        }
                    }
                }
            }

            return valid;
        }

        /// <summary>
        /// Returns a value indicating if the current text value is valid.
        /// </summary>
        /// <returns></returns>
        protected virtual bool ValidateTextInternal(string text, out string displayText)
        {
            if (this._maskChars.Count == 0)
            {
                displayText = text;
                return true;
            }

            StringBuilder displayTextBuilder = new StringBuilder(this.GetDefaultText());

            bool valid = (!string.IsNullOrEmpty(text) &&
                text.Length <= this._maskChars.Count);

            if (valid)
            {
                for (int i = 0; i < text.Length; i++)
                {
                    if (!this._maskChars[i].IsLiteral())
                    {
                        if (this.ValidateInputChar(text[i], this._maskChars[i].ValidationFlags))
                        {
                            displayTextBuilder[i] = text[i];
                        }
                        else
                        {
                            valid = false;
                        }
                    }
                }
            }

            displayText = displayTextBuilder.ToString();

            return valid;
        }

        /// <summary>
        /// Deletes the currently selected text.
        /// </summary>
        protected virtual void DeleteSelectedText()
        {
            StringBuilder text = new StringBuilder(this.Text);
            string defaultText = this.GetDefaultText();
            int selectionStart = this.SelectionStart;
            int selectionLength = this.SelectionLength;

            text.Remove(selectionStart, selectionLength);
            text.Insert(selectionStart, defaultText.Substring(selectionStart, selectionLength));
            this.Text = text.ToString();

            //reset the caret position
            this.SelectionStart = this.selectionStart = selectionStart;
        }

        /// <summary>
        /// Returns a value indicating if the specified input mask character is a placeholder.
        /// </summary>
        /// <param name="character"></param>
        /// <param name="validationFlags">If the character is a placeholder, returns the relevant validation scheme.</param>
        /// <returns></returns>
        protected virtual bool IsPlaceholderChar(char character, out InputMaskValidationFlags validationFlags)
        {
            validationFlags = InputMaskValidationFlags.None;

            switch (character.ToString().ToUpper())
            {
                case "I":
                    validationFlags = InputMaskValidationFlags.AllowInteger;
                    break;
                case "D":
                    validationFlags = InputMaskValidationFlags.AllowDecimal;
                    break;
                case "A":
                    validationFlags = InputMaskValidationFlags.AllowAlphabet;
                    break;
                case "W":
                    validationFlags = (InputMaskValidationFlags.AllowAlphanumeric);
                    break;
            }

            return (validationFlags != InputMaskValidationFlags.None);
        }

        /// <summary>
        /// Invoked when the coerce value callback is invoked.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="e"></param>
        private static void Text_CoerceValue(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            MaskedEdit mtb = (MaskedEdit)obj;
            var value = e.NewValue;
            if (value == null || value.Equals(string.Empty))
                value = mtb.GetDefaultText();
            else if (value.ToString().Length > 0)
            {
                string displayText;
                mtb.ValidateTextInternal(value.ToString(), out displayText);
                value = displayText;
            }
            mtb.Text = (string)value;
        }

        /// <summary>
        /// Invoked when the InputMask dependency property reports a change.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="e"></param>
        private static void InputMask_Changed(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            (obj as MaskedEdit).UpdateInputMask();
        }


        /// <summary>
        /// Rebuilds the InputMaskChars collection when the input mask property is updated.
        /// </summary>
        private void UpdateInputMask()
        {

            string text = this.Text;
            this._maskChars.Clear();

            this.Text = string.Empty;

            string mask = this.Mask;

            if (string.IsNullOrEmpty(mask))
                return;

            InputMaskValidationFlags validationFlags = InputMaskValidationFlags.None;

            for (int i = 0; i < mask.Length; i++)
            {
                bool isPlaceholder = this.IsPlaceholderChar(mask[i], out validationFlags);

                if (isPlaceholder)
                {
                    this._maskChars.Add(new InputMaskChar(validationFlags));
                }
                else
                {
                    this._maskChars.Add(new InputMaskChar(mask[i]));
                }
            }

            string displayText;
            if (text.Length > 0 && this.ValidateTextInternal(text, out displayText))
            {
                this.Text = displayText;
            }
            else
            {
                this.Text = this.GetDefaultText();
            }
        }

        /// <summary>
        /// Validates the specified character against its input mask validation scheme.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="validationType"></param>
        /// <returns></returns>
        private bool ValidateCharInternal(char input, InputMaskValidationFlags validationType)
        {
            bool valid = false;

            switch (validationType)
            {
                case InputMaskValidationFlags.AllowInteger:
                case InputMaskValidationFlags.AllowDecimal:
                    int i;
                    if (validationType == InputMaskValidationFlags.AllowDecimal &&
                        input == '.' && !this.Text.Contains('.'))
                    {
                        valid = true;
                    }
                    else
                    {
                        valid = int.TryParse(input.ToString(), out i);
                    }
                    break;
                case InputMaskValidationFlags.AllowAlphabet:
                    valid = char.IsLetter(input);
                    break;
                case InputMaskValidationFlags.AllowAlphanumeric:
                    valid = (char.IsLetter(input) || char.IsNumber(input));
                    break;
            }

            return valid;
        }

        /// <summary>
        /// Builds the default display text for the control.
        /// </summary>
        /// <returns></returns>
        private string GetDefaultText()
        {
            StringBuilder text = new StringBuilder();
            foreach (InputMaskChar maskChar in this._maskChars)
            {
                text.Append(maskChar.GetDefaultChar());
            }
            return text.ToString();
        }

        /// <summary>
        /// Moves the caret forward to the next non-literal position.
        /// </summary>
        private void MoveForward()
        {
            int pos = this.selectionStart;
            while (pos < this._maskChars.Count)
            {
                if (++pos == this._maskChars.Count || !this._maskChars[pos].IsLiteral())
                {
                    this.selectionStart = this.SelectionStart = pos;
                    break;
                }
            }
        }

        /// <summary>
        /// Moves the caret backward to the previous non-literal position.
        /// </summary>
        private void MoveBack()
        {
            int pos = this.selectionStart;
            while (pos > 0)
            {
                if (--pos == 0 || !this._maskChars[pos].IsLiteral())
                {
                    this.selectionStart = this.SelectionStart = pos;
                    break;
                }
            }
        }
    }
}
