//----------------------------------------------------------------------------------
//
// OPERATEUR OR
//
//----------------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
namespace RuntimeXNA.Expressions
{
	
	public class EXP_OR:CExp
	{
		public override void  evaluate(CRun rhPtr)
		{
            rhPtr.getCurrentResult().orLog(rhPtr.getNextResult());
		}
	}
}