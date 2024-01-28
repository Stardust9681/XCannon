using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace XCannon
{
				public partial class Form1 : Form
				{
								public static Bitmap CannonImage
								{
												get;
												private set;
								}

								public Color RootColour { get; set; } = Color.FromArgb(255, 45, 255, 60);
								public byte[] ARGB
								{
												get
												{
																byte[] result = new byte[4];
																result[0] = RootColour.A;
																result[1] = RootColour.R;
																result[2] = RootColour.G;
																result[3] = RootColour.B;
																return result;
												}
												set
												{
																if (value.Length != 4)
																				return;
																//textInput.Text = $"{{{value[1]}, {value[2]}, {value[3]}}}";
																RootColour = Color.FromArgb(value[0], value[1], value[2], value[3]);
												}
								}
								public PixelFormat Format
								{
												get
												{
																if (ImageText.Length < 255)
																				return PixelFormat.Format24bppRgb;
																return PixelFormat.Format16bppRgb555;
												}
								}

								public Color Shadow1
								{
												get
												{
																byte newRed = (byte)Math.Max(RootColour.R * .9f - 5, 0);
																byte newGreen = (byte)Math.Max(RootColour.G * .75f - 5, 0);
																byte newBlue = (byte)Math.Max(RootColour.B * 0.95f - 5, 0);
																return Color.FromArgb(RootColour.A, newRed, newGreen, newBlue);
												}
								}
								public Color Shadow2
								{
												get
												{
																byte newRed = (byte)Math.Max(RootColour.R * .8f - 10, 0);
																byte newGreen = (byte)Math.Max(RootColour.G * .6f - 10, 0);
																byte newBlue = (byte)Math.Max(RootColour.B * 0.85f - 10, 0);
																return Color.FromArgb(RootColour.A, newRed, newGreen, newBlue);
												}
								}
								public Color Highlight1
								{
												get
												{
																byte newRed = (byte)Math.Min(RootColour.R * 1.2f + 10, 255);
																byte newGreen = (byte)Math.Min(RootColour.G * 1.3f + 10, 255);
																byte newBlue = (byte)Math.Min(RootColour.B * 1.1f + 10, 255);
																return Color.FromArgb(RootColour.A, newRed, newGreen, newBlue);
												}
								}
								public Color Highlight2
								{
												get
												{
																byte newRed = (byte)Math.Min(RootColour.R * 1.3f + 20, 255);
																byte newGreen = (byte)Math.Min(RootColour.G * 1.45f + 20, 255);
																byte newBlue = (byte)Math.Min(RootColour.B * 1.15f + 20, 255);
																return Color.FromArgb(RootColour.A, newRed, newGreen, newBlue);
												}
								}
								public Color TextColor
								{
												get
												{
																byte Lerp(byte firstFloat, byte secondFloat, float by)
																{
																				return (byte)(((secondFloat - firstFloat) * by) + firstFloat);
																}
																Color root = RootColour;
																if (root.GetBrightness() < .48f)
																				return Color.FromArgb(Lerp(root.R, 255, 0.75f), Lerp(root.G, 255, 0.75f), Lerp(root.B, 255, 0.75f));
																return Color.FromArgb(Lerp(root.R, 0, 0.55f), Lerp(root.G, 0, 0.55f), Lerp(root.B, 0, 0.55f));
												}
								}
								public Color TextAccent
								{
												get
												{
																byte Lerp(byte firstFloat, byte secondFloat, float by)
																{
																				return (byte)(((secondFloat - firstFloat) * by) + firstFloat);
																}
																Color text = TextColor;
																Color root = RootColour;
																return Color.FromArgb(Lerp(root.R, text.R, 0.65f), Lerp(root.G, text.G, 0.65f), Lerp(root.B, text.B, 0.65f));
												}
								}
								public Color Outline1
								{
												get
												{
																byte newRed = (byte)Math.Max(RootColour.R * 0.55f - 10, 0);
																byte newGreen = (byte)Math.Max(RootColour.G * 0.5f - 10, 0);
																byte newBlue = (byte)Math.Max(RootColour.B * 0.6f - 10, 0);
																return Color.FromArgb(RootColour.A, newRed, newGreen, newBlue);
												}
								}
								public Color Outline2
								{
												get
												{
																byte newRed = (byte)Math.Max(RootColour.R * 0.35f - 15, 0);
																byte newGreen = (byte)Math.Max(RootColour.G * 0.3f - 15, 0);
																byte newBlue = (byte)Math.Max(RootColour.B * 0.4f - 15, 0);
																return Color.FromArgb(RootColour.A, newRed, newGreen, newBlue);
												}
								}
								public Color Outline3
								{
												get
												{
																byte newRed = (byte)Math.Max(RootColour.R * 0.2f - 20, 0);
																byte newGreen = (byte)Math.Max(RootColour.G * 0.15f - 20, 0);
																byte newBlue = (byte)Math.Max(RootColour.B * 0.25f - 20, 0);
																return Color.FromArgb(RootColour.A, newRed, newGreen, newBlue);
												}
								}

								public static string ImageText { get; private set; } = "KO";

								public bool TryReload(int numTrials = 3)
								{
												Exception e = default(Exception);
												for (int i = 0; i < numTrials; i++)
												{
																try
																{
																				ReloadImage();
																}
																catch (Exception x)
																{
																				e = x;
																				continue;
																}
																return true;
												}
												Logging.Log(e);
												return false;
								}
								public void ReloadImage()
								{
												if (textInput.Text.Any(x =>
												{
																try { x = x.ToString().ToUpper().First(); } catch { }
																return !CharData.ContainsKey(x);
																}))
																textInput.Text = textInput.Text.Replace(textInput.Text.First(x => {
																				try { x = x.ToString().ToUpper().First(); } catch { }
																				return !CharData.ContainsKey(x);
																}), '?');
												if (CannonImage != null)
																CannonImage.Dispose();

												int scalingFactor = ImageText.Length < 255 ? 2 : 1;

												CannonImage = new Bitmap(17 + (ImageText.Length * 4), 16);
												if (CannonImage.Width * scalingFactor > 310)
												{
																int w = 188 + (CannonImage.Width * scalingFactor);
																int screenWidth = Screen.PrimaryScreen.Bounds.Width;
																if (Bounds.Left + w > screenWidth)
																{
																				scalingFactor = 1;
																				w = 188 + CannonImage.Width;
																				if (w > screenWidth)
																								w = screenWidth;
																}
																Size = new Size(w, Size.Height);
																tableLayoutPanel1.AutoScroll = true;
																//tableLayoutPanel1.Width = Size.Width;
												}
												else
												{
																Size = new Size(500, Size.Height);
																tableLayoutPanel1.AutoScroll = false;
																//tableLayoutPanel1.Width = Size.Width;
												}
												//FillRect(new Point(3, 2), new Point(6, 2), RootColour);
												//FillRect(new Point(0, 0), new Point(19, 16), RootColour);
												for(int i = 0; i < ImageText.Length; i++)
												{
																char c = ImageText[i];
																try { c = c.ToString().ToUpper().First(); }
																catch { }
																PlaceLetter(c, new Point(11 + (i * 4), 5), (i + 1 == ImageText.Length));
												}
												HandleGrip();
												HandleMuzzle();
												CannonImage.SetPixel(9 + (ImageText.Length * 4), 10, Outline3);
												CannonImage.SetPixel(10 + (ImageText.Length * 4), 10, Outline3);
												CannonImage.SetPixel(11 + (ImageText.Length * 4), 10, Outline3);
												FillRect(new Point(11, 2), new Point(11 + (ImageText.Length * 4), 2), Outline2);

												//Size newSize = new Size(CannonImage.Width * scalingFactor, CannonImage.Height * scalingFactor);
												//pictureBox1.Image = new Bitmap(CannonImage, newSize);

												//Built-in bitmap scaling uses bilinear
												Bitmap Scale(Image current, int scaleFactor)
												{
																Graphics graphics = null;
																Bitmap newImg = new Bitmap(current.Width * scaleFactor, current.Height * scaleFactor, Format);
																try
																{
																				graphics = Graphics.FromImage(newImg);
																				graphics.Clear(Color.Black);
																				graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
																				graphics.DrawImage(current, 0, 0, current.Width * scaleFactor, current.Height * scaleFactor);
																}
																finally
																{
																				graphics?.Dispose();
																}
																return newImg;
												}
												CannonImage = Scale(CannonImage, scalingFactor);
												pictureBox1.Image = CannonImage;
								}

								private void FillRect(Point topLeft, Point botRight, Color c)
								{
												for (int i = topLeft.X; i < botRight.X; i++)
												{
																for (int j = topLeft.Y; j < botRight.Y + 1; j++)
																{
																				CannonImage.SetPixel(i, j, c);
																}
												}
								}

								//Test case:
								// ABCDEFGHIJKLMNOPQRSTUVWXYZ 1234567890 -+*/= ,.:\"|'_!? []()<> 
								static readonly IReadOnlyDictionary<char, LetterData> CharData = new Dictionary<char, LetterData>()
								{
												{'A', new LetterData('A') { CharGraphics = new int[,] { { 2, 1, 2 }, { 1, 0, 1 }, { 1, 1, 1 }, { 1, 0, 1 } } } },
												{'B', new LetterData('B') { CharGraphics = new int[,] { { 1, 1, 0 }, { 1, 1, 2 }, { 1, 0, 1 }, { 1, 1, 1 } } } },
												{'C', new LetterData('C') { CharGraphics = new int[,] { { 0, 1, 1 }, { 1, 0, 0 }, { 1, 0, 0 }, { 0, 1, 1 } } } },
												{'D', new LetterData('D') { CharGraphics = new int[,] { { 1, 1, 0 }, { 1, 0, 1 }, { 1, 0, 1 }, { 1, 1, 0 } } } },
												{'E', new LetterData('E') { CharGraphics = new int[,] { { 1, 1, 1 }, { 1, 1, 0 }, { 1, 0, 0 }, { 1, 1, 1 } } } },
												{'F', new LetterData('F') { CharGraphics = new int[,] { { 1, 1, 1 }, { 1, 1, 0 }, { 1, 0, 0 }, { 1, 0, 0 } } } },
												{'G', new LetterData('G') { CharGraphics = new int[,] { { 0, 1, 1 }, { 1, 0, 0 }, { 1, 0, 1 }, { 0, 1, 1 } } } },
												{'H', new LetterData('H') { CharGraphics = new int[,] { { 1, 0, 1 }, { 1, 1, 1 }, { 1, 0, 1 }, { 1, 0, 1 } } } },
												{'I', new LetterData('I') { CharGraphics = new int[,] { { 1, 1, 1 }, { 0, 1, 0 }, { 0, 1, 0 }, { 1, 1, 1 } } } },
												{'J', new LetterData('J') { CharGraphics = new int[,] { { 1, 1, 1 }, { 0, 0, 1 }, { 0, 0, 1 }, { 1, 1, 2 } } } },
												{'K', new LetterData('K') { CharGraphics = new int[,] { { 1, 0, 1 }, { 1, 1, 0 }, { 1, 0, 1 }, { 1, 0, 1 } } } },
												{'L', new LetterData('L') { CharGraphics = new int[,] { { 1, 0, 0 }, { 1, 0, 0 }, { 1, 0, 0 }, { 1, 1, 1 } } } },
												{'M', new LetterData('M') { CharGraphics = new int[,] { { 1, 2, 1 }, { 1, 1, 1 }, { 1, 0, 1 }, { 1, 0, 1 } } } },
												{'N', new LetterData('N') { CharGraphics = new int[,] { { 1, 1, 2 }, { 1, 0, 1 }, { 1, 0, 1 }, { 1, 0, 1 } } } },
												{'O', new LetterData('O') { CharGraphics = new int[,] { { 0, 1, 0 }, { 1, 0, 1 }, { 1, 0, 1 }, { 0, 1, 0 } } } },
												{'P', new LetterData('P') { CharGraphics = new int[,] { { 1, 1, 0 }, { 1, 0, 1 }, { 1, 1, 0 }, { 1, 0, 0 } } } },
												{'Q', new LetterData('Q') { CharGraphics = new int[,] { { 0, 1, 0 }, { 1, 0, 1 }, { 1, 1, 2 }, { 0, 1, 1 } } } },
												{'R', new LetterData('R') { CharGraphics = new int[,] { { 1, 1, 2 }, { 1, 0, 1 }, { 1, 1, 0 }, { 1, 0, 1 } } } },
												{'S', new LetterData('S') { CharGraphics = new int[,] { { 0, 1, 1 }, { 1, 2, 0 }, { 0, 2, 1 }, { 1, 1, 0 } } } },
												{'T', new LetterData('T') { CharGraphics = new int[,] { { 1, 1, 1 }, { 0, 1, 0 }, { 0, 1, 0 }, { 0, 1, 0 } } } },
												{'U', new LetterData('U') { CharGraphics = new int[,] { { 1, 0, 1 }, { 1, 0, 1 }, { 1, 0, 1 }, { 0, 1, 1 } } } },
												{'V', new LetterData('V') { CharGraphics = new int[,] { { 1, 0, 1 }, { 1, 0, 1 }, { 1, 0, 1 }, { 0, 1, 0 } } } },
												{'W', new LetterData('W') { CharGraphics = new int[,] { { 1, 0, 1 }, { 1, 0, 1 }, { 1, 1, 1 }, { 1, 2, 1 } } } },
												{'X', new LetterData('X') { CharGraphics = new int[,] { { 1, 0, 1 }, { 0, 1, 0 }, { 2, 1, 2 }, { 1, 0, 1 } } } },
												{'Y', new LetterData('Y') { CharGraphics = new int[,] { { 1, 0, 1 }, { 2, 1, 2 }, { 0, 1, 0 }, { 0, 1, 0 } } } },
												{'Z', new LetterData('Z') { CharGraphics = new int[,] { { 1, 1, 1 }, { 0, 2, 1 }, { 1, 2, 0 }, { 1, 1, 1 } } } },
												{'1', new LetterData('1') { CharGraphics = new int[,] { { 1, 1, 0 }, { 0, 1, 0 }, { 0, 1, 0 }, { 1, 1, 1 } } } },
												{'2', new LetterData('2') { CharGraphics = new int[,] { { 2, 1, 0 }, { 0, 0, 1 }, { 0, 1, 0 }, { 1, 1, 1 } } } },
												{'3', new LetterData('3') { CharGraphics = new int[,] { { 1, 1, 1 }, { 0, 1, 2 }, { 0, 0, 1 }, { 1, 1, 0 } } } },
												{'4', new LetterData('4') { CharGraphics = new int[,] { { 1, 0, 1 }, { 1, 1, 1 }, { 0, 0, 1 }, { 0, 0, 1 } } } },
												{'5', new LetterData('5') { CharGraphics = new int[,] { { 1, 1, 1 }, { 1, 1, 0 }, { 0, 0, 1 }, { 1, 1, 0 } } } },
												{'6', new LetterData('6') { CharGraphics = new int[,] { { 0, 1, 1 }, { 1, 0, 0 }, { 1, 1, 1 }, { 1, 1, 0 } } } },
												{'7', new LetterData('7') { CharGraphics = new int[,] { { 1, 1, 1 }, { 0, 0, 1 }, { 0, 1, 0 }, { 0, 1, 0 } } } },
												{'8', new LetterData('8') { CharGraphics = new int[,] { { 0, 1, 1 }, { 0, 1, 1 }, { 1, 0, 1 }, { 1, 1, 1 } } } },
												{'9', new LetterData('9') { CharGraphics = new int[,] { { 2, 1, 2 }, { 1, 0, 1 }, { 2, 1, 1 }, { 0, 0, 1 } } } },
												{'0', new LetterData('0') { CharGraphics = new int[,] { { 1, 1, 1 }, { 1, 0, 1 }, { 1, 0, 1 }, { 1, 1, 1 } } } },
												{'.', new LetterData('.') { CharGraphics = new int[,] { { 0, 0, 0 }, { 0, 0, 0 }, { 0, 0, 0 }, { 0, 1, 0 } } } },
												{',', new LetterData(',') { CharGraphics = new int[,] { { 0, 0, 0 }, { 0, 0, 0 }, { 0, 1, 0 }, { 0, 2, 0 } } } },
												{'!', new LetterData('!') { CharGraphics = new int[,] { { 0, 1, 0 }, { 0, 1, 0 }, { 0, 0, 0 }, { 0, 1, 0 } } } },
												{'?', new LetterData('?') { CharGraphics = new int[,] { { 1, 1, 2 }, { 0, 1, 2 }, { 0, 2, 0 }, { 0, 1, 0 } } } },
												{'[', new LetterData('[') { CharGraphics = new int[,] { { 1, 1, 0 }, { 1, 0, 0 }, { 1, 0, 0 }, { 1, 1, 0 } } } },
												{']', new LetterData(']') { CharGraphics = new int[,] { { 0, 1, 1 }, { 0, 0, 1 }, { 0, 0, 1 }, { 0, 1, 1 } } } },
												{'(', new LetterData('(') { CharGraphics = new int[,] { { 0, 1, 0 }, { 1, 0, 0 }, { 1, 0, 0 }, { 0, 1, 0 } } } },
												{')', new LetterData(')') { CharGraphics = new int[,] { { 0, 1, 0 }, { 0, 0, 1 }, { 0, 0, 1 }, { 0, 1, 0 } } } },
												{':', new LetterData(':') { CharGraphics = new int[,] { { 0, 0, 0 }, { 0, 1, 0 }, { 0, 0, 0 }, { 0, 1, 0 } } } },
												{'\'', new LetterData('\'') { CharGraphics = new int[,] { { 0, 1, 0 }, { 0, 2, 0 }, { 0, 0, 0 }, { 0, 0, 0 } } } },
												{'\\', new LetterData('\\') { CharGraphics = new int[,] { { 1, 0, 0 }, { 2, 1, 0 }, { 0, 1, 2 }, { 0, 0, 1 } } } },
												{'"', new LetterData('"') { CharGraphics = new int[,] { { 1, 0, 1 }, { 2, 0, 2 }, { 0, 0, 0 }, { 0, 0, 0 } } } },
												{'+', new LetterData('+') { CharGraphics = new int[,] { { 0, 0, 0 }, { 0, 1, 0 }, { 1, 1, 1 }, { 0, 1, 0 } } } },
												{'-', new LetterData('-') { CharGraphics = new int[,] { { 0, 0, 0 }, { 0, 0, 0 }, { 1, 1, 1 }, { 0, 0, 0 } } } },
												{'*', new LetterData('*') { CharGraphics = new int[,] { { 0, 0, 0 }, { 1, 0, 1 }, { 0, 1, 0 }, { 1, 0, 1 } } } },
												{'/', new LetterData('/') { CharGraphics = new int[,] { { 0, 0, 1 }, { 0, 1, 2 }, { 2, 1, 0 }, { 1, 0, 0 } } } },
												{'=', new LetterData('=') { CharGraphics = new int[,] { { 0, 0, 0 }, { 1, 1, 1 }, { 0, 0, 0 }, { 1, 1, 1 } } } },
												{'|', new LetterData('|') { CharGraphics = new int[,] { { 0, 1, 0 }, { 0, 1, 0 }, { 0, 1, 0 }, { 0, 1, 0 } } } },
												{' ', new LetterData(' ') { CharGraphics = new int[,] { { 0, 0, 0 }, { 0, 0, 0 }, { 0, 0, 0 }, { 0, 0, 0 } } } },
												{'<', new LetterData('<') { CharGraphics = new int[,] { { 0, 0, 0 }, { 0, 2, 1 }, { 1, 2, 0 }, { 0, 2, 1 } } } },
												{'>', new LetterData('>') { CharGraphics = new int[,] { { 0, 0, 0 }, { 1, 2, 0 }, { 0, 2, 1 }, { 1, 2, 0 } } } },
												{'_', new LetterData('_') { CharGraphics = new int[,] { { 0, 0, 0 }, { 0, 0, 0 }, { 0, 0, 0 }, { 1, 1, 1 } } } },
								};

								private void PlaceLetter(char c, Point position, bool isLast = false)
								{
												LetterData data = CharData[c];
												for (int i = 0; i < 3; i++)
												{
																for (int j = -2; j < 5; j++)
																{
																				Color pickColor = RootColour;
																				if (j < 0 || j > 3 || data[i, j] == 0)
																				{
																								HandleNonChar(position.X + i, position.Y + j);
																								continue;
																				}
																				else if (data[i, j] == 1)
																				{
																								pickColor = TextColor;
																				}
																				else if (data[i, j] == 2)
																				{
																								pickColor = TextAccent;
																				}
																				CannonImage.SetPixel(position.X + i, position.Y + j, pickColor);
																}
												}
												for (int i = -2; i < 5; i++)
												{
																HandleNonChar(position.X + 3, position.Y + i);
												}
												DoBottomThing(position.X, isLast);
								}
								Color IndexColour(int index)
								{
												switch (index)
												{
																case -3:
																				return Outline3;
																case -2:
																				return Outline2;
																case -1:
																				return Outline1;
																case 0:
																				return Color.Transparent;
																case 1:
																				return Shadow2;
																case 2:
																				return Shadow1;
																case 3:
																				return RootColour;
																case 4:
																				return Highlight1;
																case 5:
																				return Highlight2;
																default:
																				return Color.White;
												}
								}
								private void HandleNonChar(int posX, int posY)
								{
												int lightLevel = 3;
												if (posX < 11 || posX > CannonImage.Width - 8)
																lightLevel--;
												if (posY == 3 || posY > 7)
																lightLevel--;
												if (posY > 8)
																lightLevel--;
												if (posY == 4 || posY < 3)
																lightLevel++;
												if (lightLevel < 1)
																lightLevel = 1;
												if (lightLevel > 5)
																lightLevel = 5;
												Color lightColour = IndexColour(lightLevel);
												CannonImage.SetPixel(posX, posY, lightColour);
								}
								static readonly Array2D<int> BottomThingLeft = new Array2D<int>(new int[,] {
												{ -3, -3, -2, -3 },
												{ -1, -1, -3, -1 },
												{ -3, -3, -3, -3 },
												{ -3, -2, -2, -3 },
												{ 0, -3, -3, -3 },
								});
								static readonly Array2D<int> BottomThingRight = new Array2D<int>(new int[,] {
												{ -3, -2, -3, -3 },
												{ -2, -3, -2, -2 },
												{ -3, -3, -3, -3 },
												{ -2, -2, -2, -3 },
												{ -3, -3, -3, 0 },
								});
								private void DoBottomThing(int x, bool final = false)
								{
												for (int i = x; i < x+4; i++)
												{
																for (int j = 0; j < 5; j++)
																{
																				if (final)
																								CannonImage.SetPixel(i, CannonImage.Height - 6 + j, IndexColour(BottomThingRight[i - x, j]));
																				else
																								CannonImage.SetPixel(i, CannonImage.Height - 6 + j, IndexColour(BottomThingLeft[i - x, j]));
																}
												}
								}

								static readonly Array2D<int> Grip = new Array2D<int>(new int[,]
												{
																{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
																{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
																{ 0, 0, 0, -2, -2, -2, -2, -2, -2, -2, -1 },
																{ 0, 0, -2, 1, 2, 3, -1, -1, -2, 1, 2 },
																{ 0, -2, 1, 2, 3, 3, -1, -2, 1, 3, 3 },
																{ 0, -3, 2, 3, 4, -1, -1, -2, 3, 3, 3 },
																{ 0, -3, 3, 3, 3, -1, -1, 1, 3, 3, 3 },
																{ 0, -3, 1, 3, 3, -1, -2, 1, 3, 3, 2 },
																{ 0, -3, 1, 1, 3, -1, -2, 1, 3, 1, 1 },
																{ 0, -3, 2, 1, 1, -1, -2, 2, 2, 1, 1 },
																{ 0, 0, -3, 2, 2, -2, -2, 1, 1, -2, -2 },
																{ 0, 0, -3, -3, -3, -2, -2, -3, -3, -2, -1 },
																{ 0, -3, -2, -1, -2, -3, -3, 0, 0, 0, -2 },
																{ -3, -1, -1, -2, -3, 0, 0, 0, 0, 0, 0 },
																{ -3, -1, -2, -3, 0, 0, 0, 0, 0, 0, 0 },
																{ -3, -3, -3, 0, 0, 0, 0, 0, 0, 0, 0 },
												});

								static readonly Array2D<int> Muzzle = new Array2D<int>(new int[,]
												{
																{ 0, 0, -2, 1, 0, 0 },
																{ 0, -2, 2, 3, 1, 0 },
																{ -2, 2, 3, 2, 2, 1 },
																{ 1, 4, 4, -1, 2, 1 },
																{ 1, 3, 4, -1, 2, -1 },
																{ 1, 3, 3, -1, 2, -1 },
																{ 1, 3, 3, -1, 2, -1 },
																{ 1, 3, 3, -1, 2, -1 },
																{ 1, 3, 3, -1, 2, -1 },
																{ -1, 2, 3, -1, 2, -1 },
																{ -2, 1, 2, -1, 2, -1 },
																{ 0, -2, 1, 1, -2, 0 },
																{ 0, 0, -2, -2, 0, 0 },
																{ 0, 0, 0, 0, 0, 0 },
																{ 0, 0, 0, 0, 0, 0 },
																{ 0, 0, 0, 0, 0, 0 },
												});

								void HandleGrip()
								{
												for (int i = 0; i < 11; i++)
												{
																for (int j = 0; j < 16; j++)
																{
																				CannonImage.SetPixel(i, j, IndexColour(Grip[i, j]));
																}
												}
								}
								void HandleMuzzle()
								{
												int offset = 17 + (ImageText.Length * 4) - 6;
												for (int i = 0; i < 6; i++)
												{
																for (int j = 0; j < 16; j++)
																{
																				CannonImage.SetPixel(offset + i, j, IndexColour(Muzzle[i, j]));
																}
												}
								}

								public Form1()
								{
												InitializeComponent();
												FormBorderStyle = FormBorderStyle.FixedSingle;
												MaximizeBox = false;
												numericUpDown1.Value = trackBar1.Value = 60;
												numericUpDown2.Value = trackBar2.Value = 240;
												numericUpDown3.Value = trackBar3.Value = 45;
												splitContainer1.Capture = splitContainer2.Capture = splitContainer3.Capture = splitContainer4.Capture = false;
												splitContainer1.FixedPanel = FixedPanel.Panel2;
												splitContainer2.FixedPanel = splitContainer3.FixedPanel = splitContainer4.FixedPanel = FixedPanel.Panel1;
												splitContainer1.IsSplitterFixed = splitContainer2.IsSplitterFixed = splitContainer3.IsSplitterFixed = splitContainer4.IsSplitterFixed = false;
												ReloadImage();
								}

								private void textInput_TextChanged(object sender, EventArgs e)
								{
												int index = textInput.Text.IndexOf("\u007f");
												if (index != -1)
												{
																int start = index-1;
																for (int i = (index-1); i >= 0; i--)
																{
																				start = i;
																				if (char.IsWhiteSpace(textInput.Text[i]))
																								break;
																}
																textInput.Text = textInput.Text.Remove(start, index - start + 1);
												}
												ImageText = textInput.Text;
												if (!TryReload(3))
																Close();
								}

								private void trackBar1_Scroll(object sender, EventArgs e)
								{
												byte[] argb = ARGB;
												argb[1] = (byte)trackBar1.Value;
												numericUpDown1.Value = argb[1];
												ARGB = argb;
												TryReload(2);
								}

								private void trackBar2_Scroll(object sender, EventArgs e)
								{
												byte[] argb = ARGB;
												argb[2] = (byte)trackBar2.Value;
												numericUpDown2.Value = argb[2];
												ARGB = argb;
												TryReload(2);
								}

								private void trackBar3_Scroll(object sender, EventArgs e)
								{
												byte[] argb = ARGB;
												argb[3] = (byte)trackBar3.Value;
												numericUpDown3.Value = argb[3];
												ARGB = argb;
												TryReload(2);
								}

								private void saveButton_Click(object sender, EventArgs e)
								{
												SaveFileDialog saveDialogue = new SaveFileDialog();
												saveDialogue.Filter = "PNG Image|*png";
												saveDialogue.Title = "Save Cannon";
												saveDialogue.ShowDialog();
												if (!string.IsNullOrEmpty(saveDialogue.FileName))
												{
																if (!saveDialogue.FileName.ToLower().EndsWith(".png"))
																				saveDialogue.FileName += ".png";
																byte[] b = File.ReadAllBytes(saveDialogue.FileName);
																bool fail = false;
																using (FileStream fStream = (FileStream)saveDialogue.OpenFile())
																{
																				try
																				{
																								pictureBox1.Image.Save(fStream, System.Drawing.Imaging.ImageFormat.Png);
																				}
																				catch (Exception x)
																				{
																								Logging.Log(x);
																								fail = true;
																				}
																}
																if(fail)
																				File.WriteAllBytes(saveDialogue.FileName, b);
												}
								}

								private void copyButton_Click(object sender, EventArgs e)
								{
												try
												{
																Clipboard.SetDataObject(CannonImage);
												}
												catch { }
								}

								private void closeButton_Click(object sender, EventArgs e)
								{
												Close();
								}

								private void numericUpDown1_ValueChanged(object sender, EventArgs e)
								{
												byte[] argb = ARGB;
												argb[1] = (byte)numericUpDown1.Value;
												trackBar1.Value = argb[1];
												ARGB = argb;
												TryReload(2);
								}

								private void numericUpDown2_ValueChanged(object sender, EventArgs e)
								{
												byte[] argb = ARGB;
												argb[2] = (byte)numericUpDown2.Value;
												trackBar2.Value = argb[2];
												ARGB = argb;
												TryReload(2);
								}

								private void numericUpDown3_ValueChanged(object sender, EventArgs e)
								{
												byte[] argb = ARGB;
												argb[3] = (byte)numericUpDown3.Value;
												trackBar3.Value = argb[3];
												ARGB = argb;
												TryReload();
								}
				}
				public struct LetterData
				{
								public char Character { get; private set; }
								public int[,] CharGraphics { get; set; }
								public int this[int i, int j] => CharGraphics[j, i];
								public LetterData(char c)
								{
												Character = c;
												CharGraphics = new int[3, 4];
								}
				}
				public static class PixelState
				{
								public const int Full = 1;
								public const int Accent = 2;
								public const int None = 0;
				}
				public struct Array2D<T>
				{
								private T[,] arr;
								public ref T this[int i, int j] => ref arr[j, i];
								public Array2D(T[,] arrIn) => arr = arrIn;
								public Array2D(int len, int hig) => arr = new T[hig, len];
				}

				public static class Logging
				{
								public static void Log(object errorDetails)
								{
												SaveFileDialog logDialogue = new SaveFileDialog();
												logDialogue.Filter = "Text document|*.txt|Crashlog document|*.log";
												logDialogue.Title = "Encountered a crash: please share this log file";
												logDialogue.ShowDialog();
												if (!string.IsNullOrEmpty(logDialogue.FileName))
												{
																using (BinaryWriter writer = new BinaryWriter((FileStream)logDialogue.OpenFile()))
																{
																				string currentText = Form1.ImageText;
																				Size imageSize = Form1.CannonImage.Size;
																				writer.Write($"Text: {currentText}\n");
																				writer.Write($"Size: {imageSize.Width}, {imageSize.Height}\n");
																				writer.Write($"{errorDetails}\n");
																}
												}
								}
				}
}
