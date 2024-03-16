using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using System.Security.Cryptography;
using UnityEngine.UI;
using YamlDotNet.Core.Tokens;

public class GridUI : MonoBehaviour
{
    [MenuItem("PBTB/Gen_GridUI")]
    static void createDBT()
    {
        float xLength = 5;
        float yLength = 4;

        var bb = new PBlendTreeBuilder("Assets/Pan/GridUI");
        bb.rootDBT(() => {
            bb.add1D("1", "UIOn", () =>
            {
                bb.addMotion(0, "MUIOff");
                bb.addMotion(1, "MUIOn");
            });
            bb.add1D("1", "UIOn", () =>
            {
                bb.addDirect(0, () => bb.addMotion("InOff", "InOff"));
                bb.addDirect(1, () => bb.addMotion("In", "InOff"));
            });
            bb.add1D("1", "UIOn", () =>
            {
                bb.addDirect(0, () => bb.addMotion("InOff", "MnSender"));
                bb.addDirect(1, () => bb.addMotion("PBTB_CONST_0", "MnSender"));
            });
            bb.addDirect("UIOn", () =>
            {
                // Create Flame Count
                bb.nName("FC").add1D("1", "IFC", () => {
                    bb.addDirect(2f, () => {
                        bb.addMotion("IFC", "IFC");
                        bb.addMotion("PBTB_CONST_1", "IFC");
                        bb.addMotion("PBTB_CONST_0", "BFC0");
                        bb.addMotion("PBTB_CONST_1", "BNFC0");
                    });
                    bb.addDirect(2.1f, () => {
                        bb.addMotion("PBTB_CONST_0", "IFC");
                        bb.addMotion("PBTB_CONST_1", "BFC0");
                        bb.addMotion("PBTB_CONST_0", "BNFC0");
                    });
                });
                bb.addDirect("BFC0", null);
                bb.nName("Slide BFC").addDirect("1", () =>
                {
                    for (int i = 1; i <= 3; i++)
                    {
                        bb.addMotion($@"BFC{i - 1}", $@"BFC{i}");
                        bb.addMotion($@"BNFC{i - 1}", $@"BNFC{i}");
                    }
                });


                //Calc with not limited
                bb.nName("calc FIntegralx").addDirect("1", () => {
                    bb.add1D("1", "Fx", () =>
                    {
                        bb.addMotion(-10, "F-Integralx");
                        bb.addMotion(10, "FIntegralx");
                    });
                    bb.add1D("1", "FIntegralx", () => {
                        bb.addDirect(0, ()=> bb.addMotion("PBTB_CONST_0", "FIntegralx"));
                        bb.addDirect(xLength - 1, () => bb.addMotion($@"PBTB_CONST_{xLength - 1}", "FIntegralx"));
                    });
                });
                bb.nName("calc FIntegraly").addDirect("1", () => {
                    bb.add1D("1", "Fy", () =>
                    {
                        bb.addMotion(-10, "FIntegraly");
                        bb.addMotion(10, "F-Integraly");
                    });
                    bb.add1D("1", "FIntegraly", () => {
                        bb.addDirect(0, () => bb.addMotion("PBTB_CONST_0", "FIntegraly"));
                        bb.addDirect(xLength - 1, () => bb.addMotion($@"PBTB_CONST_{xLength - 1}", "FIntegraly"));
                    });
                });



                bb.nName("discretization_x_").add1D("BFC2", "FIntegralx", () => {
                    for (int i = 1; i < xLength; i++)
                    {
                        bb.addDirect(i - .5f - .000001f, () => bb.addMotion($@"PBTB_CONST_{i - 1}", "Ix_"));
                        bb.addDirect(i - .5f, () => bb.addMotion($@"PBTB_CONST_{i}", "Ix_"));
                    }
                });
                bb.nName("discretization_y_").add1D("BFC2", "FIntegraly", () => {
                    for (int i = 1; i < yLength; i++)
                    {
                        bb.addDirect(i - .5f - .000001f, () => bb.addMotion($@"PBTB_CONST_{i - 1}", "Iy_"));
                        bb.addDirect(i - .5f, () => bb.addMotion($@"PBTB_CONST_{i}", "Iy_"));
                    }
                });
                bb.nName("calcn_").addDirect("BFC3", () =>
                {
                    bb.addMotion("Ix_", "Ix_");
                    bb.addMotion("Iy_", "Iy_");
                    bb.addMotion("Ix_", "In_");
                    bb.addDirect($@"PBTB_CONST_{xLength}", () => bb.addMotion("Iy_", "In_"));
                });


                // ---- Calc Limit -----
                // limit constant
                bb.nName("calc yLimit").add1D("1", "InLimit", () =>
                {
                    //yLimit is the maximum y coordinate that the cursor can reach.
                    for (int i = 0; i <= yLength; i++)
                    {
                        float nowThreshold = i * xLength;
                        bb.addDirect(nowThreshold - .0001f, () => bb.addMotion($@"PBTB_CONST_{i - 1}", "IyLimit"));
                        bb.addDirect(nowThreshold, () => bb.addMotion($@"PBTB_CONST_{i}", "IyLimit"));
                    }
                });
                bb.nName("calc xLimit").addDirect("1", () =>
                {
                    //xLimit is the maximum x coordinate that the cursor can move to when y = yLimit
                    bb.addMotion("InLimit", "IxLimit");
                    bb.addDirect("IyLimit", () => bb.addMotion($@"PBTB_CONST_{xLength}", "I-xLimit"));
                });

                bb.nName("Reload xyn").addDirect("BFC0", () => {
                    bb.addMotion("Ix_", "Ix_H");
                    bb.addMotion("Iy_", "Iy_H");
                    bb.addMotion("In_", "In_H");
                });
                bb.nName("Hold xyn").addDirect("BNFC0", () => {
                    bb.addMotion("Ix_H", "Ix_H");
                    bb.addMotion("Iy_H", "Iy_H");
                    bb.addMotion("In_H", "In_H");
                });
                bb.nName("n, x subtraction").addDirect("BFC1", () => {
                    bb.addMotion("In_H", "In_Diff");
                    bb.addMotion("InLimit", "I-n_Diff");
                    bb.addMotion("Ix_H", "Ix_Diff");
                    bb.addMotion("IxLimit", "I-x_Diff");
                });
                bb.nName("n, x judgment").addDirect("BFC2", () => {
                    bb.add1D("1", "In_Diff", () => {
                        bb.addMotion(0.5f, "BNnLimitOver");
                        bb.addDirect(0.6f, () => bb.addMotion("PTBT_CONST_0", "BNnLimitOver"));
                    });
                    bb.add1D("1", "In_Diff", () => {
                        bb.addDirect(0.5f, () => bb.addMotion("PTBT_CONST_0", "BnLimitOver"));
                        bb.addMotion(0.6f, "BnLimitOver");
                    });
                    bb.add1D("1", "Ix_Diff", () =>
                    {
                        bb.addDirect(0, () => bb.addMotion("PBTB_CONST_0", "BxLimitOver"));
                        bb.addMotion(0.1f, "BxLimitOver");
                    });
                });
                bb.nName("Calculate final x, y").addDirect("BFC3", () => {
                    bb.addDirect("BNnLimitOver", () =>
                    {
                        bb.addMotion("Iy_H", "Iy");
                        bb.addMotion("Ix_H", "Ix");
                    });
                    bb.addDirect("BnLimitOver", () =>
                    {
                        bb.addMotion("IyLimit", "Iy"); // if x <= xLimit then y=yLimit
                        bb.addMotion("BxLimitOver", "I-y"); // and, if x > xLimit then y = yLimit-1
                        bb.addMotion("Ix_H", "Ix");
                    });
                });
                bb.nName("Hold x,y").addDirect("BNFC3", () => {
                    bb.addMotion("Iy", "Iy");
                    bb.addMotion("Ix", "Ix");
                });
                bb.nName("Calculate final n").addDirect("BFC0", () => {
                    bb.addMotion("Ix", "In");
                    bb.addDirect($@"PBTB_CONST_{xLength}", () => bb.addMotion("Iy", "In"));
                });
                bb.nName("Hold n").addDirect("BNFC0", () => {
                    bb.addMotion("In", "In");
                });

                //AppearCursor
                bb.nName("MgridCursor_x").addMotion("Ix", "MgridCursor_x");
                bb.nName("MgridCursor_y").addMotion("Iy", "MgridCursor_y");
                //SendResultForContact
                bb.addMotion("In", "MnSender");

                //bb.addDirect("Fn.01", null);//for AAP
                /**/
            });
        });
        bb.animatorMake();
    }
}