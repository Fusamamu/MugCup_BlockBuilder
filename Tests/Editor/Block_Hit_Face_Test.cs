using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;
using BlockBuilder.Core;

public class Block_Hit_Face_Test
{
	[Test]
	public void Check_Get_PosX_NormalFace()
	{
		NormalFace _selectedFace = BlockFaceUtil.GetSelectedFace(new Vector3(1, 0, 0));
		
		Assert.AreEqual(NormalFace.PosX, _selectedFace);
	}

	[Test] 
	public void Check_Get_NegX_NormalFace()
	{
		NormalFace _selectedFace = BlockFaceUtil.GetSelectedFace(new Vector3(-1, 0, 0));
		
		Assert.AreEqual(NormalFace.NegX, _selectedFace);
	}
	
	[Test] 
	public void Check_Get_PosZ_NormalFace()
	{
		NormalFace _selectedFace = BlockFaceUtil.GetSelectedFace(new Vector3(0, 0, 1));
		
		Assert.AreEqual(NormalFace.PosZ, _selectedFace);
	}
	
	[Test] 
	public void Check_Get_NegZ_NormalFace()
	{
		NormalFace _selectedFace = BlockFaceUtil.GetSelectedFace(new Vector3(0, 0, -1));
		
		Assert.AreEqual(NormalFace.NegZ, _selectedFace);
	}
}
